using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using BradyCodingChallenge.Model;
using BradyCodingChallenge.Model.Factors;
using BradyCodingChallenge.Model.GenerationOutput;
using BradyCodingChallenge.Model.Generators;
using System.Configuration;

namespace BradyCodingChallenge.ConsoleApp
{
   
    internal class Program
    {

        private static readonly string ProjectDirectory = Path.GetFullPath(@"..\..\..\");
        
        private const string ReferenceDataFileName = "ReferenceData";
        private const string GenerationReportFileName = "GenerationReport";
        private const string RandomTestFile = "RandomTestFile";

        private static ICollection<Day> _allDaysCollection = new List<Day>();

        private static readonly ICollection<TotalGenerationValue> AllTotalGenerationValuesCollection = new List<TotalGenerationValue>();
        private static readonly ICollection<ActualHeatRates> ActualHeatRates = new List<ActualHeatRates>();
        private static readonly ICollection<object> GeneratorCollection = new List<object>();

        public static void Main()
        {
            var appSettings = ConfigurationManager.AppSettings;

            var inputFolder = appSettings["InputFolder"];
            var outputFolder = appSettings["OutputFolder"];
            var referenceDataFolder = appSettings["ReferenceDataFolder"];
            
            var reader = new XmlReader();

            // path locations

            //var pathToReport = @$"{ProjectDirectory}\ExtraFiles\{GenerationReportFileName}.xml";
            var pathToReport = @$"{inputFolder}\{GenerationReportFileName}.xml";
            //var pathToReferenceData = $@"{ProjectDirectory}\ExtraFiles\{ReferenceDataFileName}.xml";
            var pathToReferenceData = $@"{outputFolder}\{RandomTestFile}.xml";
            // xPath string which contains all of the generators
            const string generatorXPath = "//WindGenerator|//GasGenerator|//CoalGenerator";

            // xPath string which contains all of the factors
            const string referenceDataXPath = "//ValueFactor|//EmissionsFactor";

            // factors
            var emissionFactor = new EmissionsFactor();
            var valueFactor = new ValueFactor();

            ICollection<object> generatorCollection = new List<object>();

            var doc = new XmlDocument();
            doc.Load(pathToReport);

            var referenceDataDoc = new XmlDocument();
            referenceDataDoc.Load(referenceDataFolder);

            var generators = doc.SelectNodes(generatorXPath);
            var factors = referenceDataDoc.SelectNodes(referenceDataXPath);

            emissionFactor = ParseValueAndEmissionsFactor(factors, emissionFactor, reader, ref valueFactor);
            
            PopulateGeneratorCollection(generators, GeneratorCollection, reader);
            ObjectToOutputObjects(GeneratorCollection, valueFactor, emissionFactor);

            SetDistinctAllDayCollectionWithHighestValue();
            CreateXmlFile(outputFolder);



        }

        private static void SetDistinctAllDayCollectionWithHighestValue()
        {
            _allDaysCollection = _allDaysCollection
                .GroupBy(day => day.Date)
                .Select(x => x.OrderByDescending(y => y.MaxEmissionGenerator.Emission).First())
                .ToList();
        }

        /// <summary>
        /// Sets the value of emissionsFactor and valueFactor to the data 
        /// </summary>
        /// <param name="factors">The nodes in the factor list</param>
        /// <param name="emissionFactor">EmissionsFactor object</param>
        /// <param name="reader"></param>
        /// <param name="valueFactor"></param>
        /// <returns></returns>
        private static EmissionsFactor ParseValueAndEmissionsFactor(IEnumerable factors, EmissionsFactor emissionFactor,
            XmlReader reader, ref ValueFactor valueFactor)
        {
            foreach (XmlNode node in factors)
                switch (node.Name)
                {
                    case "EmissionsFactor":
                        emissionFactor = reader.FactorNodeToObject<EmissionsFactor>(node);
                        break;
                    case "ValueFactor":
                        valueFactor = reader.FactorNodeToObject<ValueFactor>(node);
                        break;
                }

            return emissionFactor;
        }

        /// <summary>
        /// Populates the Generator Collection with the defined generator objects
        /// </summary>
        /// <param name="generators">List of generator nodes from XML-file</param>
        /// <param name="generatorCollection">collection of generator objects</param>
        /// <param name="reader"></param>
        private static void PopulateGeneratorCollection(XmlNodeList generators, ICollection<object> generatorCollection, XmlReader reader)
        {
            foreach (XmlNode node in generators)
                switch (node.Name)
                {
                    case "WindGenerator":
                        generatorCollection.Add(reader.GeneratorNodeToList<WindGenerator>(node));
                        break;
                    case "GasGenerator":
                        generatorCollection.Add(reader.GeneratorNodeToList<GasGenerator>(node));
                        break;
                    case "CoalGenerator":
                        generatorCollection.Add(reader.GeneratorNodeToList<CoalGenerator>(node));
                        break;
                    default:
                        throw new NotImplementedException();
                }
        }
        
        // TODO: Change method name to something that makes more sense
        private static void ObjectToOutputObjects(IEnumerable<object> generatorCollection, Factor valueFactor, Factor emissionFactor)
        {
            foreach (var generator in generatorCollection)
            {
                var name = (string) GetPropValue(generator, "Name");
                var days = (ICollection<Day>) GetPropValue(generator, "Days");


                /*
                 *  Offshore Wind:  ValueFactor(Low), EmissionFactor(N/A)
                 *  Onshore Wind:   ValueFactor(High), EmissionFactor(N/A)
                 *  Gas:            ValueFactor(Medium), EmissionFactor(Medium)
                 *  Coal:           ValueFactor(Medium), EmissionFactor(High)
                 */
                var selectedValueFactor = GetValueFactorForGenerator(valueFactor, name);
                var selectedEmissionFactor = GetEmissionFactorForGenerator(emissionFactor, name);

                    
                var totalGenerationValue = new TotalGenerationValue();

                foreach (var day in days)
                {
                    totalGenerationValue.Total += day.Energy * day.Price * selectedValueFactor;

                    if (selectedEmissionFactor == 0) continue;

                    var emissionRating = (double) GetPropValue(generator, "EmissionsRating");
                    var highestDailyEmission = day.Energy * emissionRating * selectedEmissionFactor;

                    day.MaxEmissionGenerator = new MaxEmissionGenerator(name, highestDailyEmission);
                    _allDaysCollection.Add(day);
                }


                totalGenerationValue.Name = name;
                AllTotalGenerationValuesCollection.Add(totalGenerationValue);


                if (!name.Contains("Coal")) continue;

                //var totalHeatInput = (double) GetPropValue(generator, "TotalHeatInput");

                var gen = (CoalGenerator) generator;
                var actualHeatRates = gen.TotalHeatInput / gen.ActualNetGeneration;

                gen.ActualHeatRates = new ActualHeatRates(gen.Name, actualHeatRates);

                ActualHeatRates.Add(gen.ActualHeatRates);
            }
        }


        private static double GetValueFactorForGenerator(Factor valueFactor, string name)
        {
            var selectedValueFactor = name switch
            {
                "Wind[Onshore]" => valueFactor.High,
                "Wind[Offshore]" => valueFactor.Low,
                "Gas[1]" => valueFactor.High,
                "Coal[1]" => valueFactor.Medium,
                _ => 0
            };

            return selectedValueFactor;
        }

        private static double GetEmissionFactorForGenerator(Factor emissionFactor, string name)
        {
            var selectedEmissionFactor = name switch
            {
                "Gas[1]" => emissionFactor.Medium,
                "Coal[1]" => emissionFactor.High,
                _ => 0
            };

            return selectedEmissionFactor;
        }

        public static void CreateXmlFile(string secondPath)
        {
            XmlElement generatorElement;
            XmlElement nameElement;
            XmlElement totalElement;
            XmlElement dayElement;
            XmlElement dateElement;
            XmlElement emissionElement;
            XmlElement heatRateElement;

            XmlText nameText;
            XmlText dateText;
            XmlText totalText;
            XmlText emissionText;
            XmlText heatRatesText;

            var newDoc = new XmlDocument();
            var xmlDeclaration = newDoc.CreateXmlDeclaration("1.0", "UTF-8", null);

            var root = newDoc.DocumentElement;
            newDoc.InsertBefore(xmlDeclaration, root);

            var generationOutputElement = newDoc.CreateElement(string.Empty, "GenerationOutput", string.Empty);
            newDoc.AppendChild(generationOutputElement);

            var totalsElement = CreateNewElementAndAppendChild(newDoc, "GenerationOutput", generationOutputElement);
            var maxEmissionGeneratorsNode =
                CreateNewElementAndAppendChild(newDoc, "MaxEmissionGenerators", generationOutputElement);


            AllTotalGenerationValuesCollection.ToList().ForEach(x =>
            {
                generatorElement = CreateNewElementAndAppendChild(newDoc, "Generator", totalsElement);
                nameElement = CreateNewElementAndAppendChild(newDoc, "Name", generatorElement);
                totalElement = CreateNewElementAndAppendChild(newDoc, "Total", generatorElement);

                nameText = newDoc.CreateTextNode(x.Name);
                totalText = newDoc.CreateTextNode(x.Total.ToString(CultureInfo.InvariantCulture));

                nameElement.AppendChild(nameText);
                totalElement.AppendChild(totalText);
            });

            _allDaysCollection.ToList().ForEach(x =>
            {
                dayElement = CreateNewElementAndAppendChild(newDoc, "Day", maxEmissionGeneratorsNode);
                nameElement = CreateNewElementAndAppendChild(newDoc, "Name", dayElement);
                dateElement = CreateNewElementAndAppendChild(newDoc, "Date", dayElement);
                emissionElement = CreateNewElementAndAppendChild(newDoc, "Emission", dayElement);

                nameText = newDoc.CreateTextNode(x.MaxEmissionGenerator.Name);
                dateText = newDoc.CreateTextNode(x.Date.ToString(CultureInfo.InvariantCulture));
                emissionText = newDoc.CreateTextNode(x.MaxEmissionGenerator.Emission.ToString(CultureInfo.InvariantCulture));

                nameElement.AppendChild(nameText);
                dateElement.AppendChild(dateText);
                emissionElement.AppendChild(emissionText);
            });

            var actualHeatRatesElement = CreateNewElementAndAppendChild(newDoc, "ActualHeatRates", generationOutputElement);

            ActualHeatRates.ToList().ForEach(x =>
            {
                generatorElement = CreateNewElementAndAppendChild(newDoc, "Generator", actualHeatRatesElement);
                nameElement = CreateNewElementAndAppendChild(newDoc, "Name", generatorElement);
                heatRateElement = CreateNewElementAndAppendChild(newDoc, "HeatRate", generatorElement);

                nameText = newDoc.CreateTextNode(x.Name);
                heatRatesText = newDoc.CreateTextNode(x.HeatRate.ToString(CultureInfo.InvariantCulture));

                nameElement.AppendChild(nameText);
                heatRateElement.AppendChild(heatRatesText);
            });

          
            //newDoc.Save(@$"{ProjectDirectory}\Output\{RandomTestFile}.xml");
            newDoc.Save($@"{secondPath}\{RandomTestFile}.xml");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="localName"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static XmlElement CreateNewElementAndAppendChild(XmlDocument doc, string localName, XmlElement parent)
        {
            var newElement = doc.CreateElement(string.Empty, localName, string.Empty);
            parent.AppendChild(newElement);
            return newElement;
        }

        public static void PrintCollectionMembers<T>(ICollection<T> collection)
        {
            foreach (var childObject in collection)
            {
                Console.WriteLine(childObject);
            }
        }

        /// <summary>
        ///     Gets the property value.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static object GetPropValue(object src, string propertyName)
        {
            return src.GetType().GetProperty(propertyName)?.GetValue(src, null);
        }
    }
}