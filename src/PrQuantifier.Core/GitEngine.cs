namespace PrQuantifier.Core
***REMOVED***
    using System.Collections.Generic;
    using System.Linq;
    using LibGit2Sharp;

    public sealed class GitEngine
    ***REMOVED***
        /// <summary>
        /// Get the change counts in terms of lines.
        /// </summary>
        /// <param name="path">Path to any item in the repository.</param>
        /// <returns>Dictionary of counts by operation type.</returns>
        public IDictionary<GitOperationType, int> GetGitChangeCounts(string path)
        ***REMOVED***
            var repoRoot = Repository.Discover(path);
            using var repo = new Repository(repoRoot);

            var trackedFilesPatch = repo.Diff.Compare<Patch>();
            var linesAdded = trackedFilesPatch.LinesAdded;
            var linesDeleted = trackedFilesPatch.LinesDeleted;

            var status = repo.RetrieveStatus(new StatusOptions
            ***REMOVED***
                IncludeUntracked = true,
                RecurseUntrackedDirs = true,
                IncludeIgnored = false
    ***REMOVED***);
            if (status.Untracked.Any())
            ***REMOVED***
                var untrackedFilesPatch = repo.Diff.Compare<Patch>(
                    status.Untracked.Select(u => u.FilePath), true, new ExplicitPathsOptions());
                linesAdded += untrackedFilesPatch.LinesAdded;
                linesDeleted += untrackedFilesPatch.LinesDeleted;
    ***REMOVED***

            return new Dictionary<GitOperationType, int>
            ***REMOVED***
                ***REMOVED*** GitOperationType.Add, linesAdded ***REMOVED***,
                ***REMOVED*** GitOperationType.Delete, linesDeleted ***REMOVED***
    ***REMOVED***;
***REMOVED***
***REMOVED***
***REMOVED***
