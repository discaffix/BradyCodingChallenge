namespace BradyCodingChallenge.Model.Generators
{
    public class WindGenerator : Generator
    {
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public WindGenerator() { }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the generator</param>
        public WindGenerator(string name)
            : base(name)
        { }

        /// <summary>
        /// Location of the generator
        /// </summary>
        public string Location { get; set; }
    }
}
