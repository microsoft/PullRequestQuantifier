namespace PrQuantifier.Core.Model
{
    using System.Collections.Generic;

    public sealed class Threshold
    {
        /// <summary>
        /// GitHub operation type.
        /// </summary>
        public List<GitOperationType> GitOperationType { get; set; }

        /// <summary>
        /// The upper bound threshold.
        /// The value is expressed as percentile score.
        /// </summary>
        public short Value { get; set; }

        /// <summary>
        /// The label name we want to output.
        /// </summary>
        public string Label { get; set; }
    }
}
