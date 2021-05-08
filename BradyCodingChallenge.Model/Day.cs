using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BradyCodingChallenge.Model.GenerationOutput;

namespace BradyCodingChallenge.Model
{
    public class Day : IEnumerable
    {
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public Day() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="date">Date of the record</param>
        /// <param name="energy">Energy record</param>
        /// <param name="price">Price record</param>
        public Day(DateTime date, double energy, double price)
        {
            Date = date;
            Energy = energy;
            Price = price;
        }

        /// <summary>
        /// Date of which record was recorded
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Energy record of the day
        /// </summary>
        public double Energy { get; set; }

        /// <summary>
        /// Price record of the day
        /// </summary>
        public double Price { get; set; }

        public MaxEmissionGenerator MaxEmissionGenerator { get; set; }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{Date}: [{MaxEmissionGenerator.Name}] {MaxEmissionGenerator.Emission}";
        }
    }
}
