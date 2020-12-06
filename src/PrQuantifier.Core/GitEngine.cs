namespace PrQuantifier.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using LibGit2Sharp;

    public sealed class GitEngine
    {
        /// <summary>
        /// Get the change counts in terms of lines.
        /// </summary>
        /// <param name="path">Path to any item in the repository.</param>
        /// <returns>Dictionary of counts by operation type.</returns>
        public IDictionary<GitOperationType, int> GetGitChangeCounts(string path)
        {
            var repoRoot = Repository.Discover(path);
            using var repo = new Repository(repoRoot);

            var trackedFilesPatch = repo.Diff.Compare<Patch>();
            var linesAdded = trackedFilesPatch.LinesAdded;
            var linesDeleted = trackedFilesPatch.LinesDeleted;

            var status = repo.RetrieveStatus(new StatusOptions
            {
                IncludeUntracked = true,
                RecurseUntrackedDirs = true,
                IncludeIgnored = false
            });
            if (status.Untracked.Any())
            {
                var untrackedFilesPatch = repo.Diff.Compare<Patch>(
                    status.Untracked.Select(u => u.FilePath), true, new ExplicitPathsOptions());
                linesAdded += untrackedFilesPatch.LinesAdded;
                linesDeleted += untrackedFilesPatch.LinesDeleted;
            }

            return new Dictionary<GitOperationType, int>
            {
                { GitOperationType.Add, linesAdded },
                { GitOperationType.Delete, linesDeleted }
            };
        }
    }
}
