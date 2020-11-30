namespace PrQuantifier
{
    using System.Collections.Generic;

    public class QuantifierResult
    {
        /// <summary>
        /// The output category as quantified. This is a
        /// number less than NumCategories in the QuantifierOptions.
        /// Smallest size is Category 0.
        /// </summary>
        public int Category { get; internal set; }

        /// <summary>
        /// Map of change counts by operation type as identifed
        /// by the quantifier.
        /// </summary>
        public IDictionary<OperationType, int> ChangeCounts { get; internal set; }
    }
}
