using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using BradyCodingChallenge.Model.GenerationOutput;

namespace BradyCodingChallenge.Model
{
    public class Generator
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Generator() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Generator(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the generator
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Total Emission Per
        /// </summary>
        public MaxEmissionGenerator MaxEmissionGenerator{ get; set; }

        /// <summary>
        /// The daily report of the Power Generation
        /// </summary>
        /// <value>
        /// The days.
        /// </value>
        public ICollection<Day> Days { get; set; } = new List<Day>();

    }
}
