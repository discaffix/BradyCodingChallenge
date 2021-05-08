using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BradyCodingChallenge.Model.GenerationOutput
{
    public class ActualHeatRates
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="heatRate"></param>
        public ActualHeatRates(string name, double heatRate)
        {
            HeatRate = heatRate;
            Name = name;
        }


        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double HeatRate { get; set; }
    }
}
