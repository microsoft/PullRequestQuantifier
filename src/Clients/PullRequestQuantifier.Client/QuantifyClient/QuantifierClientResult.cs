namespace PullRequestQuantifier.Client.QuantifyClient
{
    using System.Collections.Generic;
    using global::PullRequestQuantifier.Abstractions.Context;

    public sealed class QuantifierClientResult
    {
        /// <summary>
        /// Gets or sets the output label as quantified.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the output color as quantified.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the formula based onn which  we will combine the additions and deletions.
        /// </summary>
        [System.Text.Json.Serialization.JsonConverterAttribute(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
        public ThresholdFormula Formula { get; set; }

        /// <summary>
        /// Gets the output explanation.
        /// </summary>
        public string Explanation { get; internal set; }

        /// <summary>
        /// Gets or sets the quantified total number of lines added in this diff.
        /// Will be determined in the specific context.
        /// </summary>
        public int QuantifiedLinesAdded { get; set; }

        /// <summary>
        /// Gets or sets the quantified total number of lines deleted in this diff.
        /// Will be determined in the specific context.
        /// </summary>
        public int QuantifiedLinesDeleted { get; set; }

        /// <summary>
        /// Gets or sets the deletion percentile within the context for this quantified results.
        /// </summary>
        public float PercentileDeletion { get; set; }

        /// <summary>
        /// Gets or sets the addition percentile within the context for this quantified results.
        /// </summary>
        public float PercentileAddition { get; set; }

        /// <summary>
        /// Gets or sets based on the QuantifyClientOutput different kind of details.
        /// </summary>
        public IEnumerable<dynamic> Details { get; set; }
    }
}
