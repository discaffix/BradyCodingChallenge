using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;
using BradyCodingChallenge.Model;
using static System.Double;

namespace BradyCodingChallenge.ConsoleApp
{
    internal class XmlReader
    {
        private ICollection<Day> _listOfDays;
        
        /// <summary>
        /// Sets the value of a property with a generic type
        /// </summary>
        /// <param name="node">Node object with all of the information about the node</param>
        /// <param name="item">Generator object</param>
        /// <param name="propertyName">Property name of the generator object</param>
        /// <param name="valObj">Property of the generator object</param>
        public static void SetValue<T>([Optional] XmlNode node, T item, [Optional] string propertyName, [Optional] ICollection<Day> valObj) where T : class
        {
            var propInfo = item.GetType().GetProperty(propertyName ?? node.Name);
            propInfo?.SetValue(item, valObj ?? Convert.ChangeType(node.InnerText, propInfo.PropertyType), null);
        }

        /// <summary>
        /// Converts the generator node and all of its children to objects
        /// </summary>
        /// <typeparam name="T">Type of generator</typeparam>
        /// <param name="node">Generator node from XmlDocument</param>
        /// <returns></returns>
        internal T GeneratorNodeToList<T>(XmlNode node) where T : class
        {
            var obj = (T)Activator.CreateInstance(typeof(T));

            // loops through all of the properties of the generator
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "Generation")
                {
                    _listOfDays = new List<Day>();

                    foreach (XmlNode day in child.ChildNodes)
                    {
                        var dayObject = new Day();
                        
                        // Set the correct objects for the day
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
                                default:
                                    throw new NotImplementedException();
                            }
                        }

                        _listOfDays.Add(dayObject);
                    }

                    SetValue(item: obj, propertyName: "Days", valObj: _listOfDays);
                }
                else
                    SetValue(child, obj);
            }

            return obj;
        }

        /// <summary>
        /// returns an object of the corresponding factor node
        /// </summary>
        /// <param name="node">Factor node</param>
        /// <returns></returns>
        public virtual T FactorNodeToObject<T>(XmlNode node) where T : class
        {
            var obj = (T)Activator.CreateInstance(typeof(T));

            foreach (XmlNode child in node.ChildNodes)
                SetValue(child, obj);
            
            return obj;
        }
    }
}
