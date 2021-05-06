using System;
using System.Collections;
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
using static System.Double;

namespace BradyCodingChallenge.ConsoleApp
{
    internal class Program
    {
        public static void Main()
        {
            ICollection<WindGenerator> windGenerators = new List<WindGenerator>();
            ICollection<GasGenerator> gasGenerators = new List<GasGenerator>();
            ICollection<CoalGenerator> coalGenerators = new List<CoalGenerator>();

            foreach (var windGenerator in windGenerators)
            {
                Console.WriteLine(windGenerator.Name);
            }

            foreach (var gasGenerator in windGenerators)
            {
                Console.WriteLine(gasGenerator.Name);
            }

            foreach (var coalGenerator in coalGenerators)
            {
                Console.WriteLine(coalGenerator.Name);
            }

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
                        windGenerators.Add(GetListItemFromXml<WindGenerator>(node));
                        break;
                    case "GasGenerator":
                        gasGenerators.Add(GetListItemFromXml<GasGenerator>(node));
                        break;
                    case "CoalGenerator":
                        coalGenerators.Add(GetListItemFromXml<CoalGenerator>(node));
                        break;
                }

            }

            //Sample(elemList);
            Console.ReadKey();

        }
        
        
        public static T GetListItemFromXml<T>(XmlNode node) where T : class
        {
            var obj = (T) Activator.CreateInstance(typeof(T), new object[] { });

            // loops through all of the properties of the generator
            foreach (XmlNode child in node.ChildNodes)
            {
                Console.WriteLine(child.Name);

                if (child.Name != "Generation")
                {

                    var propertyInfo = obj.GetType().GetProperty(child.Name);
                    propertyInfo.SetValue(obj, Convert.ChangeType(child.InnerText, propertyInfo.PropertyType), null);
                }
                else
                {
                    Debug.Assert(child.FirstChild != null, "child.FirstChild != null");

                    ICollection<Day> days = new List<Day>();
                    
                    // loops through the days 
                    foreach (XmlNode day in child.ChildNodes)
                    {
                        Day dayObject = new Day();

                        foreach (XmlNode prop in day.ChildNodes)
                        {
                            switch (prop.Name)
                            {
                                case "Date":
                                    dayObject.Date = DateTime.Parse(prop.InnerText);
                                    break;
                                case "Energy":
                                    dayObject.Energy = Parse(prop.InnerText);
                                    break;
                                case "Price":
                                    dayObject.Price = Parse(prop.InnerText);
                                    break;
                            }
                        }
                        
                        days.Add(dayObject);
                    }

                    var propertyInfo = obj.GetType().GetProperty("Days");
                    propertyInfo.SetValue(obj, days, null);
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
