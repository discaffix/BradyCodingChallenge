using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BradyCodingChallenge.Model.Generators
{
    public class GasGenerator : Generator
    {
        public GasGenerator() { }
        public GasGenerator(string name, double emissionRating)
            : base(name, emissionRating)
        { }

        public override string ToString()
        {
            return $"Name: {Name}, Emission Rating: {EmissionsRating} ";
        }
    }
}
