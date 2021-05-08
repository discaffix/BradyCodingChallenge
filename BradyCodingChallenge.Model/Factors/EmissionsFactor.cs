namespace BradyCodingChallenge.Model.Factors
{
    public class EmissionsFactor : Factor
    {
        public EmissionsFactor(){ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="low"></param>
        /// <param name="medium"></param>
        /// <param name="high"></param>
        public EmissionsFactor(double low, double medium, double high) 
            : base(low, medium, high)
        { }
    }
}
