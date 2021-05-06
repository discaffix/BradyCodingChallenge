using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BradyCodingChallenge.Model
{
    public class ThermalGenerator : Generator
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public ThermalGenerator(){ }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the generator</param>
        /// <param name="emissionsRating"></param>
        public ThermalGenerator(string name, double emissionsRating)
            :base(name)
        {
            EmissionsRating = emissionsRating;
        }
        
        /// <summary>
        /// Generator emission rating
        /// </summary>
        public double EmissionsRating { get; set; }
    }
}
