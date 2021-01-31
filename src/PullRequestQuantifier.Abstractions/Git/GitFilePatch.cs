namespace PullRequestQuantifier.Abstractions.Git
{
    using System.Collections.Generic;
    using System.Linq;
    using PullRequestQuantifier.Abstractions.Git.DiffParser;
    using PullRequestQuantifier.Abstractions.Git.DiffParser.Models;

    public sealed class GitFilePatch
    {
        private string diffContent;

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
        public string DiffContent
        {
            get => diffContent;
            set
            {
                diffContent = value;
                FileDiff = DiffParserHelper.Parse(value).First();
                DiffLines = FileDiff.Chunks.SelectMany(c => c.Changes);
            }
        }

        /// <summary>
        /// Gets or sets the path of the changed file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the file extension.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the kind of change reported by diff.
        /// </summary>
        public GitChangeType ChangeType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether discard from counting.
        /// In case we want to remove this file from the final counting mark this as true.
        /// </summary>
        public bool DiscardFromCounting { get; set; }

        /// <summary>
        /// Parsed line diffs.
        /// Used for change counting.
        /// Only for internal use.
        /// </summary>
        internal IEnumerable<LineDiff> DiffLines { get; set; }

        /// <summary>
        /// Contains parsed value of <see cref="DiffContent"/>.
        /// Only for internal use.
        /// </summary>
        private FileDiff FileDiff { get; set; }
    }
}