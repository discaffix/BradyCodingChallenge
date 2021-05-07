namespace BradyCodingChallenge.Model.Factors
{
    public class EmissionFactor : Factor
    {
        public EmissionFactor(){ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="low"></param>
        /// <param name="medium"></param>
        /// <param name="high"></param>
        public EmissionFactor(double low, double medium, double high) 
            : base(low, medium, high)
        { }
    }
}
