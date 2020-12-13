namespace PrQuantifier
{
    public sealed class QuantifierResult
    {
        /// <summary>
        /// Gets the output label as quantified.
        /// </summary>
        public string Label { get; internal set; }

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
        /// Gets quantifier input.
        /// </summary>
        public QuantifierInput QuantifierInput { get; internal set; }
    }
}
