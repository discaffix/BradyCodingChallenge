namespace BradyCodingChallenge.Model.Factors
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
