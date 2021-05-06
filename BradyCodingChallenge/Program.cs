using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using BradyCodingChallenge.Model.Generators;
using BradyCodingChallenge.Model.NewFolder;

namespace BradyCodingChallenge.ConsoleApp
{
    internal class Program
    {
        public static void Main()
        {
            var reader = new XmlReader();
            
            var projectDirectory = Path.GetFullPath(@"..\..\..\");

            var referenceDataFileName = "ReferenceData";
            var generationReportFileName = "GenerationReport";

            var pathToReport = @$"{projectDirectory}\ExtraFiles\{generationReportFileName}.xml";
            var pathToReferenceData = $@"{projectDirectory}\ExtraFiles\{referenceDataFileName}.xml";
            
            var generatorXPath = "//WindGenerator|//GasGenerator|//CoalGenerator";
            var referenceDataXPath = "//ValueFactor|//EmissionFactor";
            
            var emissionFactor = new EmissionFactor();
            var valueFactor = new ValueFactor();

            ICollection<WindGenerator> windGenerators = new List<WindGenerator>();
            ICollection<GasGenerator> gasGenerators = new List<GasGenerator>();
            ICollection<CoalGenerator> coalGenerators = new List<CoalGenerator>();

            var doc = new XmlDocument();
            doc.Load(pathToReport);

            var referenceDataDoc = new XmlDocument();
            referenceDataDoc.Load(pathToReferenceData);

            var generators = doc.SelectNodes(generatorXPath);
            var factors = referenceDataDoc.SelectNodes(referenceDataXPath);

            Debug.Assert(factors != null, nameof(factors) + " != null");
            foreach (XmlNode node in factors)
            {
                switch (node.Name)
                {
                    case "EmissionFactor":
                        emissionFactor = reader.FactorNodeToObject<EmissionFactor>(node);
                        break;
                    case "ValueFactor":
                        valueFactor = reader.FactorNodeToObject<ValueFactor>(node);
                        break;
                }
            }

            Debug.Assert(generators != null, nameof(generators) + " != null");
            foreach (XmlNode node in generators)
            {
                switch (node.Name)
                {
                    case "WindGenerator":
                        windGenerators.Add(reader.GeneratorNodeToList<WindGenerator>(node));
                        break;
                    case "GasGenerator":
                        gasGenerators.Add(reader.GeneratorNodeToList<GasGenerator>(node));
                        break;
                    case "CoalGenerator":
                        coalGenerators.Add(reader.GeneratorNodeToList<CoalGenerator>(node));
                        break;
                }
            }

            foreach (var windGenerator in windGenerators)
            {
                Console.WriteLine($"Daily Generation Value for: {windGenerator.Name}");

                foreach (var daily in windGenerator.Days)
                {
                    var selectedValueFactor = windGenerator.Name.Contains("Onshore") 
                        ? valueFactor.High : valueFactor.Low;

                    Console.WriteLine($"{daily.Date}");
                    Console.WriteLine($"\tDaily Generation Value");
                    Console.WriteLine($"\t\t{daily.Energy*daily.Price*selectedValueFactor}\n");
                }
                
            }
        }
    }
}
