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
        /// 
        /// </summary>
        public ThermalGenerator(){}

        public ThermalGenerator(string name, double emissionsRating)
            :base(name)
        {
            EmissionsRating = emissionsRating;
        }
        
        public double EmissionsRating { get; set; }
    }
}
