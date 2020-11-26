namespace PrQuantifier
{
    using System.Collections.Generic;

    public class QuantifierOptions
    {
        /// <summary>
        /// List of regex based paths/filenames to include.
        /// Supports .gitignore type patterns.
        /// </summary>
        public IEnumerable<string> Included { get; set; }

        /// <summary>
        /// List of regex based paths/filenames to exclude.
        /// Supports .gitignore type patterns.
        /// </summary>
        public IEnumerable<string> Excluded { get; set; }

        /// <summary>
        /// Number of categories that can be output by the
        /// quantifier. This number must match the number of
        /// thresholds configured for each operation type.
        /// </summary>
        public int NumCategories { get; set; }

        /// <summary>
        /// Map of category thresholds by operation type. The size
        /// of each threshold list must match the number of
        /// categories defined.
        /// </summary>
        public IDictionary<OperationType, IEnumerable<int>> Thresholds { get; set; }

        // more options like ignore whitespaces
    }
}
