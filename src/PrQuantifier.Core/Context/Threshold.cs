namespace PrQuantifier.Core.Context
{
    using System.Collections.Generic;
    using PrQuantifier.Core.Git;

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
    }
}
