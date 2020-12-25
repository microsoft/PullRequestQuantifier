namespace PrQuantifier.GitEngine
***REMOVED***
    using System.Collections.Generic;
    using LibGit2Sharp;
    using PrQuantifier.Abstractions.Git;

    public interface IGitEngine
    ***REMOVED***
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

        /// <summary>
        /// Get all historical changes for this particular path to Parent branch.
        /// </summary>
        /// <param name="path">The path to the repository.</param>
        /// <returns>returns a collection with changes.
        /// string is the sha1 of the commit and values represents the changes.</returns>
        IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> GetGitHistoricalChangesToParent(string path);

        /// <summary>
        /// Gets a repo root from a path.
        /// </summary>
        /// <param name="path">The path to the repository.</param>
        /// <returns>returns repo root.</returns>
        string GetRepoRoot(string path);
***REMOVED***
***REMOVED***
