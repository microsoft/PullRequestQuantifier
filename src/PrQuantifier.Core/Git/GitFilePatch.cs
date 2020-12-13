namespace PrQuantifier.Core.Git
{
    public sealed class GitFilePatch
    {
        /// <summary>
        /// Gets or sets the absolute total number of lines added in this diff.
        /// </summary>
        public int AbsoluteLinesAdded { get; set; }

        /// <summary>
        /// Gets or sets the absolute total number of lines deleted in this diff.
        /// </summary>
        public int AbsoluteLinesDeleted { get; set; }

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
        /// Gets or sets the full patch file of this diff.
        /// </summary>
        public string DiffContent { get; set; }

        /// <summary>
        /// Gets or sets the path of the changed file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the kind of change reported by diff.
        /// </summary>
        public GitChangeType ChangeType { get; set; }
    }
}
