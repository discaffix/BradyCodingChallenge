namespace BradyCodingChallenge.Model.Generators
{
    public class WindGenerator : Generator
    {
        /// <summary>
        /// 
        /// </summary>
        public WindGenerator() { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public WindGenerator(string name)
            : base(name)
        { }


        /// <summary>
        /// 
        /// </summary>
        public string Location { get; set; }
    }
}
