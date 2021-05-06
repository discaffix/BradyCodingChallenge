using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BradyCodingChallenge.Model
{
    public class Day
    {
        public Day() { }

        public Day(DateTime date, double energy, double price)
        {
            Date = date;
            Energy = energy;
            Price = price;
        }
        [DataType(DataType.Date)]
        [DisplayFormat]
        public DateTime Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Energy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Price { get; set; }


    }
}
