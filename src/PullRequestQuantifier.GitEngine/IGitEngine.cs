﻿namespace PullRequestQuantifier.GitEngine
{
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using LibGit2Sharp;
    using PullRequestQuantifier.Abstractions.Git;

    public interface IGitEngine
    {
        /// <summary>
        /// Get the git changes.
        /// </summary>
        /// <param name="path">Path to any item in the repository.</param>
        /// <param name="commitSha1">Sha1 of the commit (example 78d1ffd2b6fbcdf2b5f278df0c22bb09a0971c0b).</param>
        /// <returns>Dictionary of counts by operation type.</returns>
        public IEnumerable<GitFilePatch> GetGitChange(
            string path,
            string commitSha1);

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
        /// Create a repository.
        /// </summary>
        /// <param name="repositoryPath">The repository path.</param>
        /// <returns>returns filesystem handler used for creating the repository.</returns>
        FileSystem CreateRepository(string repositoryPath);

        /// <summary>
        /// Gets a repo root from a path.
        /// </summary>
        /// <param name="path">The path to the repository.</param>
        /// <returns>returns repo root.</returns>
        string GetRepoRoot(string path);
    }
}
