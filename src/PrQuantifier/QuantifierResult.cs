namespace PrQuantifier
***REMOVED***
    using System.Collections.Generic;

    public class QuantifierResult
    ***REMOVED***
        /// <summary>
        /// The output category as quantified. This is a
        /// number less than NumCategories in the QuantifierOptions.
        /// Smallest size is Category 0.
        /// </summary>
        public int Category ***REMOVED*** get; internal set; ***REMOVED***

        /// <summary>
        /// Map of change counts by operation type as identifed
        /// by the quantifier.
        /// </summary>
        public IDictionary<OperationType, int> ChangeCounts ***REMOVED*** get; internal set; ***REMOVED***
***REMOVED***
***REMOVED***
