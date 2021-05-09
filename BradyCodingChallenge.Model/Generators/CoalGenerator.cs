using System.Reflection;
using BradyCodingChallenge.Model.GenerationOutput;

namespace BradyCodingChallenge.Model.Generators
{
    public class CoalGenerator : ThermalGenerator
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public CoalGenerator() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoalGenerator"/> class.
        /// </summary>
        /// <param name="name">The name of the coal generator.</param>
        /// <param name="totalHeatInput">The total heat input.</param>
        /// <param name="actualNetGeneration">The actual net generation.</param>
        /// <param name="emissionRating">The emission rating.</param>
        public CoalGenerator(string name, double totalHeatInput, double actualNetGeneration, double emissionRating)
            : base(name, emissionRating)
        {
            TotalHeatInput = totalHeatInput;
            ActualNetGeneration = actualNetGeneration;
        }

        /// <summary>
        /// Gets or sets the total heat input.
        /// </summary>
        /// <value>
        /// The total heat input.
        /// </value>
        public double TotalHeatInput { get; set; }

        /// <summary>
        /// Gets or sets the actual net generation.
        /// </summary>
        /// <value>
        /// The actual net generation.
        /// </value>
        public double ActualNetGeneration { get; set; }


        /// <summary>
        /// Heat rates of a Coal Engine
        /// </summary>
        public ActualHeatRates ActualHeatRates { get; set; }
    }
}
