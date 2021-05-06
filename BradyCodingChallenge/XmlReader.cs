﻿using System;
using System.Collections.Generic;
using System.Reflection;
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">The </param>
        /// <param name="item">Generator object</param>
        /// <param name="propertyName">Property of the generator object</param>
        /// <param name="valObj">Property of the generator object</param>
        private static void SetValue<T>([Optional] XmlNode node, T item, [Optional] string propertyName, [Optional] ICollection<Day> valObj) where T : class
        {
            var propInfo = item.GetType().GetProperty(propertyName ?? node.Name);
            propInfo?.SetValue(item, valObj ?? Convert.ChangeType(node.InnerText, propInfo.PropertyType), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
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

                    // loops through the days 
                    foreach (XmlNode day in child.ChildNodes)
                    {
                        var dayObject = new Day();
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

                        _listOfDays.Add(dayObject);
                    }

                    SetValue(item: obj, propertyName: "Days", valObj: _listOfDays);
                }
                else
                    SetValue(child, obj);
            }

            return obj;
        }
    }
}
