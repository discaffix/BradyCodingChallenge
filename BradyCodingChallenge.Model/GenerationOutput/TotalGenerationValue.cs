namespace BradyCodingChallenge.Model.GenerationOutput
{
    public class TotalGenerationValue
    {
        public TotalGenerationValue() { }
        public TotalGenerationValue(string name, double total)
        {
            Name = name;
            Total = total;
        }

        public string Name { get; set; }
        public double Total { get; set; }


        public override string ToString()
        {
            return $"{Name}: {Total}";
        }
    }
}
