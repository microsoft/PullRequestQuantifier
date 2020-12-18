namespace PrQuantifier.Core.Git
***REMOVED***
    public sealed class GitFilePatch
    ***REMOVED***
        /// <summary>
        /// Gets or sets the absolute total number of lines added in this diff.
        /// </summary>
        public int AbsoluteLinesAdded ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets the absolute total number of lines deleted in this diff.
        /// </summary>
        public int AbsoluteLinesDeleted ***REMOVED*** get; set; ***REMOVED***

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
        /// Gets or sets the full patch file of this diff.
        /// </summary>
        public string DiffContent ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets the DiffContentLines.
        /// This will store lines used for final counting.
        /// </summary>
        public string[] DiffContentLines ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets the path of the changed file.
        /// </summary>
        public string FilePath ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets the kind of change reported by diff.
        /// </summary>
        public GitChangeType ChangeType ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// Gets or sets a value indicating whether discard from counting.
        /// In case we want to remove this file from the final counting mark this as true.
        /// </summary>
        public bool DiscardFromCounting ***REMOVED*** get; set; ***REMOVED***
***REMOVED***
***REMOVED***
