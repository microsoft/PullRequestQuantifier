namespace PrQuantifier.Abstractions.Context
***REMOVED***
    using System.Collections.Generic;
    using PrQuantifier.Abstractions.Git;

    public sealed class Threshold
    ***REMOVED***
        /// <summary>
        /// Gets  or sets gitHub operation type.
        /// </summary>
        public IEnumerable<GitOperationType> GitOperationType ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets  or sets the upper bound threshold.
        /// The value is expressed as percentile score.
        /// </summary>
        public short Value ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets  or sets the label name we want to output.
        /// </summary>
        public string Label ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets  or sets the color we want to output.
        /// </summary>
        public string Color ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets the formula based onn which  we will combine the additions and deletions.
        /// </summary>
        public ThresholdFormula Formula ***REMOVED*** get; set; ***REMOVED***
***REMOVED***
***REMOVED***
