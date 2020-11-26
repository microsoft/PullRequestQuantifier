namespace PrQuantifier
***REMOVED***
    using System.Collections.Generic;

    public class QuantifierOptions
    ***REMOVED***
        /// <summary>
        /// List of regex based paths/filenames to include.
        /// Supports .gitignore type patterns.
        /// </summary>
        public IEnumerable<string> Included ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// List of regex based paths/filenames to exclude.
        /// Supports .gitignore type patterns.
        /// </summary>
        public IEnumerable<string> Excluded ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Number of categories that can be output by the
        /// quantifier. This number must match the number of
        /// thresholds configured for each operation type.
        /// </summary>
        public int NumCategories ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Map of category thresholds by operation type. The size
        /// of each threshold list must match the number of
        /// categories defined.
        /// </summary>
        public IDictionary<OperationType, IEnumerable<int>> Thresholds ***REMOVED*** get; set; ***REMOVED***

        // more options like ignore whitespaces
***REMOVED***
***REMOVED***
