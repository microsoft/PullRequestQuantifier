namespace PrQuantifier.Abstractions.Context
{
    using System.Collections.Generic;
    using PrQuantifier.Abstractions.Git;

    public sealed class Threshold
    {
        /// <summary>
        /// Gets  or sets gitHub operation type.
        /// </summary>
        public IEnumerable<GitOperationType> GitOperationType { get; set; }

        /// <summary>
        /// Gets  or sets the upper bound threshold.
        /// The value is expressed as percentile score.
        /// </summary>
        public short Value { get; set; }

        /// <summary>
        /// Gets  or sets the label name we want to output.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets  or sets the color we want to output.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the formula based onn which  we will combine the additions and deletions.
        /// </summary>
        public ThresholdFormula Formula { get; set; }
    }
}
