namespace PullRequestQuantifier.Abstractions.Context
{
    using System.Collections.Generic;
    using PullRequestQuantifier.Abstractions.Git;

    public sealed class Context
    {
        /// <summary>
        /// Gets  or sets <see cref="Included"/>.
        /// Will have a list of expressions (paths, files similar to gitignore)
        /// to filter out all other files not part of the included list.
        /// If this will be empty everything will be included.
        /// Supports .gitignore type patterns.
        /// </summary>
        public IEnumerable<string> Included { get; set; }

        /// <summary>
        /// Gets  or sets <see cref="Excluded"/>.
        /// Will have a list of expressions (paths, files similar to gitignore)
        /// to filter out all files part of the excluded list.
        /// This is a reverse of <see cref="Included"/>.
        /// The idea of having both included and excluded is to allow users,
        /// in case not all file extensions or paths are known, to only include specific paths/extensions
        /// (for example cc, cs extensions), in which case the excluded list will be treated like an empty list.
        /// Supports .gitignore type patterns.
        /// </summary>
        public IEnumerable<string> Excluded { get; set; }

        /// <summary>
        /// Gets or sets <see cref="GitOperationType"/>.
        /// If empty all operations will be considered,
        /// otherwise if something specified.
        /// </summary>
        public IEnumerable<GitOperationType> GitOperationType { get; set; }

        /// <summary>
        /// Gets or sets thresholds.
        /// Thresholds for this model.
        /// </summary>
        public IEnumerable<Threshold> Thresholds { get; set; }

        /// <summary>
        /// Gets or sets language specific options when we quantify.
        /// </summary>
        public LanguageOptions LanguageOptions { get; set; }

        /// <summary>
        /// Gets or sets the DynamicBehaviour value.
        /// This setting will allow the behaviour to be adjusted based on the previous
        /// local git merge history.
        /// </summary>
        public bool DynamicBehaviour { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should ignore the renamed files.
        /// </summary>
        public bool IgnoreRenamed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should ignore the copied files.
        /// </summary>
        public bool IgnoreCopied { get; set; }

        /// <summary>
        /// Gets  or sets AdditionPercentile. Used in quantifier final computation.
        /// </summary>
        public SortedDictionary<int, float> AdditionPercentile { get; set; }

        /// <summary>
        /// Gets  or sets DeletionPercentile. Used in quantifier final computation.
        /// </summary>
        public SortedDictionary<int, float> DeletionPercentile { get; set; }

        /// <summary>
        /// Gets  or sets FormulaPercentile. Used in quantifier final computation.
        /// </summary>
        public IEnumerable<(ThresholdFormula, SortedDictionary<int, float>)> FormulaPercentile { get; set; }
    }
}