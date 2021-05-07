using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BradyCodingChallenge.Model.GenerationOutput
{
    public class TotalGenerationValue
    {
        public TotalGenerationValue(double total)
        {
            Total = total;
        }

        
        public double Total{ get; set; }
    }
}
