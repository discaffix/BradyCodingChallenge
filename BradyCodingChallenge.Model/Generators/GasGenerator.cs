using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BradyCodingChallenge.Model.Generators
{
    public class GasGenerator : ThermalGenerator
    {
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public GasGenerator() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="emissionRating"></param>
        public GasGenerator(string name, double emissionRating)
            : base(name, emissionRating)
        { }
        
        /// <summary>
        /// Returns a string when object is called
        /// </summary>
        /// <returns>Name, and emission rating</returns>
        public override string ToString()
        {
            return $"Name: {Name}, Emission Rating: {EmissionsRating} ";
        }
    }
}
