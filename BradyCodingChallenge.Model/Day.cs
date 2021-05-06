using System;
using System.Collections.Generic;
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

        public DateTime Date { get; set; }

        public double Energy { get; set; }

        public double Price { get; set; }


    }
}
