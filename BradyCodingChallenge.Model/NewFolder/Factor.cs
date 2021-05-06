using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BradyCodingChallenge.Model.NewFolder
{ 
    public class Factor
    {
        public Factor() {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="low"></param>
        /// <param name="medium"></param>
        /// <param name="high"></param>
        public Factor(double low, double medium, double high)
        {
            Low = low;
            Medium = medium;
            High = high;
        }

        public double Low { get; set; }

        public double Medium { get; set; }

        public double High { get; set; }
    }
}
