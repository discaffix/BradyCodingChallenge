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
        public static void Main()
        {
            var doc = new XmlDocument();
            var projectDirectory = GetCurrentDirectory();

            doc.Load(@$"{projectDirectory}\ExtraFiles\GenerationReport.xml");
            var generators = doc.SelectNodes("//WindGenerator|//GasGenerator|//CoalGenerator");

            Debug.Assert(generators != null, nameof(generators) + " != null");

            foreach (XmlNode node in generators)
            {
                Console.WriteLine(node.Name + "\n");
                
                switch(node.Name)
                {
                    case "WindGenerator":
                        GetListItemFromXml<WindGenerator>(node);
                        break;
                    case "GasGenerator":
                        break;
                    case "CoalGenerator":
                        break;
                }

            }

            //Sample(elemList);
            Console.ReadKey();

        }
        
        public static T GetListItemFromXml<T>(XmlNode node) where T : class
        {
            var obj = (T) Activator.CreateInstance(typeof(T), new object[] { });

            foreach (XmlNode child in node.ChildNodes)
            {
                Console.WriteLine(child.Name);

                if (child.Name != "Generation")
                {
                    var propertyInfo = obj.GetType().GetProperty(child.Name);
                    propertyInfo.SetValue(obj, Convert.ChangeType(child.InnerText, propertyInfo.PropertyType), null);
                }

            }

            return obj;
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
