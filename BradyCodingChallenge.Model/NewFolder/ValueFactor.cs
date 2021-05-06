using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BradyCodingChallenge.Model.NewFolder
{
    public class ValueFactor : Factor
    {
        public ValueFactor() { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="low"></param>
        /// <param name="medium"></param>
        /// <param name="high"></param>
        public ValueFactor(double low, double medium, double high) 
            : base(low, medium, high)
        { }
    }
}
