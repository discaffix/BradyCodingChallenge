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
        private const string RandomTestFile = "GenerationOutput";

        private static ICollection<Day> _allDaysCollection = new List<Day>();
        private static ICollection<TotalGenerationValue> _allTotalGenerationValuesCollection = new List<TotalGenerationValue>();
        private static ICollection<ActualHeatRates> _actualHeatRates = new List<ActualHeatRates>();
        private static ICollection<object> _generatorCollection = new List<object>();

        public static void Main()
        {
            var appSettings = ConfigurationManager.AppSettings;
            var inputFolder = appSettings["InputFolder"];


            if (!Directory.Exists(inputFolder))
                Directory.CreateDirectory(inputFolder ?? throw new InvalidOperationException());

            var watcher = new FileSystemWatcher
            {
                Path = inputFolder ?? throw new InvalidOperationException(), NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size, Filter = "*.xml"
            };

            watcher.Created += OnCreated;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Put a file in the Input Folder to parse it, or press enter to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// Triggered whenever a new .xml file has been added to the input folder
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Created)
                return;
            
            const string generatorXPath = "//WindGenerator|//GasGenerator|//CoalGenerator";
            const string referenceDataXPath = "//ValueFactor|//EmissionsFactor";
            
            var appSettings = ConfigurationManager.AppSettings;
            var outputFolder = appSettings["OutputFolder"];
            var referenceDataFolder = appSettings["ReferenceDataFolder"];
            var inputFolder = appSettings["InputFolder"];

            var reader = new XmlReader();
            var pathToReport = e.FullPath;
            
            //redeclare lists to empty them
            _allTotalGenerationValuesCollection = new List<TotalGenerationValue>();
            _actualHeatRates = new List<ActualHeatRates>();
            _generatorCollection = new List<object>();

            var emissionFactor = new EmissionsFactor();
            var valueFactor = new ValueFactor();

            ICollection<object> generatorCollection = new List<object>();
            
            var doc = new XmlDocument();
            var referenceDataDoc = new XmlDocument();
            try 
            {
                doc.Load(pathToReport);
                referenceDataDoc.Load(referenceDataFolder!);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            var generators = doc.SelectNodes(generatorXPath);
            var factors = referenceDataDoc.SelectNodes(referenceDataXPath);
            var oldReportsFolder = @$"{inputFolder}\OldReports";

            if (!Directory.Exists(oldReportsFolder))
                Directory.CreateDirectory(oldReportsFolder);

            emissionFactor = ParseValueAndEmissionsFactor(factors, emissionFactor, reader, ref valueFactor);

            PopulateGeneratorCollection(generators, _generatorCollection, reader);
            ObjectListToOutputObjects(_generatorCollection, valueFactor, emissionFactor);

            SetDistinctAllDayCollectionWithHighestValue();
            CreateXmlFile(outputFolder);

            try
            {


                var newFileName = GetNewFileName(e.FullPath);
                if (!File.Exists(e.FullPath)) return;
                
                File.Move(e.FullPath, @$"{oldReportsFolder}\{newFileName}.xml");
                Console.WriteLine(@$"Moved {e.Name} to \ReadReports");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Get the new name of the file that is to be stored
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns>new file name</returns>
        private static string GetNewFileName(string fullPath)
        {
            var currentDate = DateTime.UtcNow;
            var unixTimeInMilliseconds = new DateTimeOffset(currentDate).ToUnixTimeMilliseconds();
            var newFileName = $"{Path.GetFileNameWithoutExtension(fullPath)}-{unixTimeInMilliseconds}";
            return newFileName;
        }

        /// <summary>
        ///  Select the day with the highest emission, and filter out the rest
        /// </summary>
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
                    default:
                        throw new NotImplementedException();
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="generatorCollection">Collection of generators</param>
        /// <param name="valueFactor">ValueFactor of the object</param>
        /// <param name="emissionFactor">EmissionFactor of the object</param>
        private static void ObjectListToOutputObjects(IEnumerable<object> generatorCollection, Factor valueFactor, Factor emissionFactor)
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
                _allTotalGenerationValuesCollection.Add(totalGenerationValue);

                GetActualHeatRatesForCoalGenerator(name, generator);
            }
        }

        private static void GetActualHeatRatesForCoalGenerator(string name, object generator)
        {
            if (!name.Contains("Coal")) return;
            var gen = (CoalGenerator) generator;
            var actualHeatRates = gen.TotalHeatInput / gen.ActualNetGeneration;

            gen.ActualHeatRates = new ActualHeatRates(gen.Name, actualHeatRates);
            _actualHeatRates.Add(gen.ActualHeatRates);
        }

        /// <summary>
        /// Get the Value Factor for generators
        /// </summary>
        /// <param name="valueFactor"></param>
        /// <param name="name"></param>
        /// <returns>ValueFactor for specified generator</returns>
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

        /// <summary>
        /// Get the Emission Factor for generator
        /// </summary>
        /// <param name="emissionFactor"></param>
        /// <param name="name"></param>
        /// <returns>EmissionsValue for specified generator</returns>
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

        /// <summary>
        /// Create Xml File with data from output objects
        /// </summary>
        /// <param name="outputPath">path to the output string</param>
        public static void CreateXmlFile(string outputPath)
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

            var totalsElement = CreateNewElementAndAppendChild(newDoc, "Totals", generationOutputElement);
            var maxEmissionGeneratorsNode =
                CreateNewElementAndAppendChild(newDoc, "MaxEmissionGenerators", generationOutputElement);

            _allTotalGenerationValuesCollection.ToList().ForEach(x =>
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

            var oldGenerationFolder = @$"{outputPath}\OldGenerationOutput";

            var latestFile = $@"{outputPath}\{RandomTestFile}.xml";
            var currentDate = DateTime.UtcNow;
            var unixTimeInMilliseconds = new DateTimeOffset(currentDate).ToUnixTimeMilliseconds();

            try
            {
                if (!Directory.Exists(outputPath))
                    Directory.CreateDirectory(outputPath);
                
                if (File.Exists(latestFile))
                {
                    if (!Directory.Exists(oldGenerationFolder))
                        Directory.CreateDirectory(oldGenerationFolder);

                    var destinationFileName = $@"{outputPath}\OldGenerationOutput\{RandomTestFile}-{unixTimeInMilliseconds}.xml";
                    File.Move(latestFile, destinationFileName);
                }
                
                newDoc.Save($@"{outputPath}\{RandomTestFile}.xml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        ///     Creates a new document element, and assigns it to a parent defined through parent parameter
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="localName">Name of element</param>
        /// <param name="parent">Parent element</param>
        /// <returns>New element</returns>
        public static XmlElement CreateNewElementAndAppendChild(XmlDocument doc, string localName, XmlElement parent)
        {
            var newElement = doc.CreateElement(string.Empty, localName, string.Empty);
            parent.AppendChild(newElement);
            return newElement;
        }
        
        /// <summary>
        ///     Returns the value of a property if it exists
        /// </summary>
        /// <param name="src">Source object</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Returns the value of a property</returns>
        public static object GetPropValue(object src, string propertyName)
        {
            return src.GetType().GetProperty(propertyName)?.GetValue(src, null);
        }
    }
}
