namespace BradyCodingChallenge.Model.Factors
{
    public class EmissionsFactor : Factor
    {
        public EmissionsFactor() { }
       
        /// <summary>
        /// Constructor
        /// </summary>
        public EmissionsFactor(double low, double medium, double high) 
            : base(low, medium, high)
        { }
    }
}
