namespace PrQuantifier.Core.Abstractions
{
    using System.Collections.Generic;
    using LibGit2Sharp;
    using PrQuantifier.Core.Git;

    public interface IGitEngine
    {
        /// <summary>
        /// Get the git changes.
        /// </summary>
        /// <param name="path">Path to any item in the repository.</param>
        /// <returns>Dictionary of counts by operation type.</returns>
        public IEnumerable<GitFilePatch> GetGitChanges(string path);

        /// <summary>
        /// Get all commits for a particular path.
        /// </summary>
        /// <param name="path">The path to the repository.</param>
        /// <returns>returns a collection of commits.</returns>
        IEnumerable<Commit> GetAllCommits(string path);
    }
}
