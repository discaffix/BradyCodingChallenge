using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace BradyCodingChallenge.Model
{
    public class Generator
    {
        public Generator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Generator(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="emissionsRating">The emissions rating.</param>
        public Generator(string name, double emissionsRating)
        {
            Name = name;
            EmissionsRating = emissionsRating;
        }

        public string Name { get; set; }

        public double EmissionsRating { get; set; }

        /// <summary>
        /// The daily report of the Power Generation
        /// </summary>
        /// <value>
        /// The days.
        /// </value>
        public Collection<Day> Days { get; } = new Collection<Day>();

        /// <summary>
        /// Adds 
        /// </summary>
        /// <param name="day">The day.</param>
        public void AddDay(Day day)
        {
            Days.Add(day);
        }
    }
}
