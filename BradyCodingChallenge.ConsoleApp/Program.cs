using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using BradyCodingChallenge.Model;
using BradyCodingChallenge.Model.Generators;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace BradyCodingChallenge.ConsoleApp
{
    internal class Program
    {
        public static int counter = 0;
        public static void Main()
        {
            var doc = new XmlDocument();
            var projectDirectory = GetCurrentDirectory();

            doc.Load(@$"{projectDirectory}\ExtraFiles\GenerationReport.xml");
            var windGenerators = doc.SelectNodes("/GenerationReport/Wind/WindGenerator");


            //Sample(elemList);
            Console.ReadKey();
        }

        /// <summary>
        /// Gets the current directory.
        /// </summary>
        /// <returns>
        /// The path 
        /// </returns>
        public static string GetCurrentDirectory()
        {
            return Path.GetFullPath(@"..\..\..\");
        }

        public static void Sample(XmlNode parentNode)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                Console.WriteLine($"{node.Name} - {node.Value}");
                Sample(node);
            }
        }
    }
}
