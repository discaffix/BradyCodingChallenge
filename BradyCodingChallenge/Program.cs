using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using BradyCodingChallenge.Model;
using BradyCodingChallenge.Model.Factors;
using BradyCodingChallenge.Model.GenerationOutput;
using BradyCodingChallenge.Model.Generators;

namespace BradyCodingChallenge.ConsoleApp
{
    internal class Program
    {
        private static readonly ICollection<Day> AllDaysCollection = new List<Day>();

        private static readonly ICollection<TotalGenerationValue> AllTotalGenerationValuesCollection
            = new List<TotalGenerationValue>();

        public static void Main()
        {
            var reader = new XmlReader();

            var projectDirectory = Path.GetFullPath(@"..\..\..\");

            const string referenceDataFileName = "ReferenceData";
            const string generationReportFileName = "GenerationReport";

            var pathToReport = @$"{projectDirectory}\ExtraFiles\{generationReportFileName}.xml";
            var pathToReferenceData = $@"{projectDirectory}\ExtraFiles\{referenceDataFileName}.xml";

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

                //Console.WriteLine($"{name}: {totalGenerationValue}");

            }

            PrintCollectionMembers(AllDaysCollection);
            PrintCollectionMembers(AllTotalGenerationValuesCollection);
        }
        //
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