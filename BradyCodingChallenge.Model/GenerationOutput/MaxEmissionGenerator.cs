using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BradyCodingChallenge.Model.GenerationOutput
{
    public class MaxEmissionGenerator : Generator
    {
        public MaxEmissionGenerator(string name, DateTime date, double emission)
            : base(name)
        {
            Emission = emission;
        }
        
        public double Emission { get; set; }
    }
}
