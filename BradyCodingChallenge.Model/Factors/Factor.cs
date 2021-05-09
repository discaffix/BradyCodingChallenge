namespace BradyCodingChallenge.Model.Factors
{ 
    public class Factor
    {
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

        public Factor() {}

        public double Low { get; set; }

        public double Medium { get; set; }

        /// <summary>
        /// Gets or sets the high.
        /// </summary>
        /// <value>
        /// The high.
        /// </value>
        public double High { get; set; }
    }
}
