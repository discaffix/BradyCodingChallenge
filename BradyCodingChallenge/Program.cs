using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using BradyCodingChallenge.Model;
using BradyCodingChallenge.Model.Factors;
using BradyCodingChallenge.Model.GenerationOutput;
using BradyCodingChallenge.Model.Generators;

namespace BradyCodingChallenge.ConsoleApp
{
   
    internal class Program
    {
        private static string _projectDirectory = Path.GetFullPath(@"..\..\..\");

        private const string _referenceDataFileName = "ReferenceData";
        private const string _generationReportFileName = "GenerationReport";
        private const string _randomTestFile = "RandomTestFile";

        private static ICollection<Day> AllDaysCollection = new List<Day>();

        private static readonly ICollection<TotalGenerationValue> AllTotalGenerationValuesCollection
            = new List<TotalGenerationValue>();

        private static ICollection<ActualHeatRates> _actualHeatRates = new List<ActualHeatRates>();

        public static void Main()
        {
            var reader = new XmlReader();

            
            var pathToReport = @$"{_projectDirectory}\ExtraFiles\{_generationReportFileName}.xml";
            var pathToReferenceData = $@"{_projectDirectory}\ExtraFiles\{_referenceDataFileName}.xml";

            const string generatorXPath = "//WindGenerator|//GasGenerator|//CoalGenerator";
            const string referenceDataXPath = "//ValueFactor|//EmissionsFactor";

            var emissionFactor = new EmissionsFactor();
            var valueFactor = new ValueFactor();

            ICollection<object> generatorCollection = new List<object>();

            var doc = new XmlDocument();
            doc.Load(pathToReport);

            var referenceDataDoc = new XmlDocument();
            referenceDataDoc.Load(pathToReferenceData);

            var generators = doc.SelectNodes(generatorXPath);
            var factors = referenceDataDoc.SelectNodes(referenceDataXPath);

            Debug.Assert(factors != null, nameof(factors) + " != null");
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

            Debug.Assert(generators != null, nameof(generators) + " != null");
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
                        throw new Exception();
                }

            foreach (var generator in generatorCollection)
            {
                var name = (string) GetPropValue(generator, "Name");
                var days = (ICollection<Day>) GetPropValue(generator, "Days");

                double selectedEmissionFactor = 0;
                double selectedValueFactor = 0;

                /*
                 *  Offshore Wind:  ValueFactor(Low), EmissionFactor(N/A)
                 *  Onshore Wind:   ValueFactor(High), EmissionFactor(N/A)
                 *  Gas:            ValueFactor(Medium), EmissionFactor(Medium)
                 *  Coal:           ValueFactor(Medium), EmissionFactor(High)
                 */
                switch (name)
                {
                    case "Wind[Onshore]":
                        selectedValueFactor = valueFactor.High;
                        break;
                    case "Wind[Offshore]":
                        selectedValueFactor = valueFactor.Low;
                        break;
                    case "Gas[1]":
                        selectedValueFactor = valueFactor.High;
                        selectedEmissionFactor = emissionFactor.Medium;
                        break;
                    case "Coal[1]":
                        selectedValueFactor = valueFactor.Medium;
                        selectedEmissionFactor = emissionFactor.High;
                        break;
                }

                var totalGenerationValue = new TotalGenerationValue();

                foreach (var day in days)
                {
                    totalGenerationValue.Total += day.Energy * day.Price * selectedValueFactor;

                    if (selectedEmissionFactor == 0) continue;

                    var emissionRating = (double) GetPropValue(generator, "EmissionsRating");
                    var highestDailyEmission = day.Energy * emissionRating * selectedEmissionFactor;

                    day.MaxEmissionGenerator = new MaxEmissionGenerator(name, highestDailyEmission);
                    AllDaysCollection.Add(day);
                    
                }




                totalGenerationValue.Name = name;
                AllTotalGenerationValuesCollection.Add(totalGenerationValue);


                if (!name.Contains("Coal")) continue;

                //var totalHeatInput = (double) GetPropValue(generator, "TotalHeatInput");

                var gen = (CoalGenerator)generator;
                var actualHeatRates = gen.TotalHeatInput / gen.ActualNetGeneration;

                gen.ActualHeatRates = new ActualHeatRates(gen.Name, actualHeatRates);

                _actualHeatRates.Add(gen.ActualHeatRates);
            }

            // selects the day with highest emission
            AllDaysCollection = AllDaysCollection
                .GroupBy(day => day.Date)
                .Select(x => x.OrderByDescending(y => y.MaxEmissionGenerator.Emission).
                    First())
                .ToList();
            

            //PrintCollectionMembers(highestDailyEmissions);
            //PrintCollectionMembers(AllTotalGenerationValuesCollection);
            PrintCollectionMembers(_actualHeatRates);
            CreateXmlFile();
        }

        public static void CreateXmlFile()
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

            AllDaysCollection.ToList().ForEach(x =>
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

            _actualHeatRates.ToList().ForEach(x =>
            {
                generatorElement = CreateNewElementAndAppendChild(newDoc, "Generator", actualHeatRatesElement);
                nameElement = CreateNewElementAndAppendChild(newDoc, "Name", generatorElement);
                heatRateElement = CreateNewElementAndAppendChild(newDoc, "HeatRate", generatorElement);

                nameText = newDoc.CreateTextNode(x.Name);
                heatRatesText = newDoc.CreateTextNode(x.HeatRate.ToString(CultureInfo.InvariantCulture));

                nameElement.AppendChild(nameText);
                heatRateElement.AppendChild(heatRatesText);
            });

          
            newDoc.Save(@$"{_projectDirectory}\Output\{_randomTestFile}.xml");
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
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