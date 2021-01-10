namespace PullRequestQuantifier.Abstractions.Core
{
    using System.Collections.Generic;
    using PullRequestQuantifier.Abstractions.Git;

    public sealed class QuantifierInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuantifierInput"/> class.
        /// </summary>
        public QuantifierInput()
        {
            Changes = new List<GitFilePatch>();
        }

        /// <summary>
        /// Gets changed files containing info about the diff.
        /// </summary>
        public List<GitFilePatch> Changes { get; }
    }
}
