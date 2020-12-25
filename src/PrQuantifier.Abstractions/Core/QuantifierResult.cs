namespace PrQuantifier.Abstractions.Core
***REMOVED***
    using PrQuantifier.Abstractions.Context;

    public sealed class QuantifierResult
    ***REMOVED***
        /// <summary>
        /// Gets or sets the output label as quantified.
        /// </summary>
        public string Label ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets the output color as quantified.
        /// </summary>
        public string Color ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets the formula based onn which  we will combine the additions and deletions.
        /// </summary>
        [System.Text.Json.Serialization.JsonConverterAttribute(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
        public ThresholdFormula Formula ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets the output explanation.
        /// </summary>
        public string Explanation ***REMOVED*** get; internal set; ***REMOVED***

        /// <summary>
        /// Gets or sets the quantified total number of lines added in this diff.
        /// Will be determined in the specific context.
        /// </summary>
        public int QuantifiedLinesAdded ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets the quantified total number of lines deleted in this diff.
        /// Will be determined in the specific context.
        /// </summary>
        public int QuantifiedLinesDeleted ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets the deletion percentile within the context for this quantified results.
        /// </summary>
        public float PercentileDeletion ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets the addition percentile within the context for this quantified results.
        /// </summary>
        public float PercentileAddition ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets the quantifier input.
        /// </summary>
        public QuantifierInput QuantifierInput ***REMOVED*** get; set; ***REMOVED***
***REMOVED***
***REMOVED***
