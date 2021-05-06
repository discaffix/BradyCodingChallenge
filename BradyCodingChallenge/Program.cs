using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using BradyCodingChallenge.Model;
using BradyCodingChallenge.Model.Generators;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using static System.Double;

namespace BradyCodingChallenge.ConsoleApp
{
    internal class Program
    {
        public static void Main()
        {
            var reader = new XmlReader();

            var projectDirectory = Path.GetFullPath(@"..\..\..\");
            var pathToReport = @$"{projectDirectory}\ExtraFiles\GenerationReport.xml";
            var generatorXPath = "//WindGenerator|//GasGenerator|//CoalGenerator";

            ICollection<WindGenerator> windGenerators = new List<WindGenerator>();
            ICollection<GasGenerator> gasGenerators = new List<GasGenerator>();
            ICollection<CoalGenerator> coalGenerators = new List<CoalGenerator>();

            var doc = new XmlDocument();
            doc.Load(pathToReport);

            var generators = doc.SelectNodes(generatorXPath);
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
                Console.WriteLine(windGenerator.Name);
            }

            foreach (var gasGenerator in gasGenerators)
            {
                Console.WriteLine(gasGenerator.Name);
            }

            foreach (var coalGenerator in coalGenerators)
            {
                Console.WriteLine(coalGenerator.Name);
            }

            //Sample(elemList);
            Console.ReadKey();

        }
    }
}
