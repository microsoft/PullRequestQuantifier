namespace PrQuantifier.Core.Git
***REMOVED***
    using System.Collections.Generic;
    using System.Linq;
    using LibGit2Sharp;

    public sealed class GitEngine
    ***REMOVED***
        private static readonly StatusOptions RepoStatusOptions = new StatusOptions
        ***REMOVED***
            IncludeUntracked = true,
            RecurseUntrackedDirs = true,
            IncludeIgnored = false
***REMOVED***;

        /// <summary>
        /// Get the git changes.
        /// </summary>
        /// <param name="path">Path to any item in the repository.</param>
        /// <returns>Dictionary of counts by operation type.</returns>
        public IEnumerable<GitFilePatch> GetGitChanges(string path)
        ***REMOVED***
            var ret = new List<GitFilePatch>();
            var repoRoot = Repository.Discover(path);
            using var repo = new Repository(repoRoot);

            var trackedFilesPatch = repo.Diff.Compare<Patch>();
            ret.AddRange(GetGitFilePatch(trackedFilesPatch));

            var status = repo.RetrieveStatus(RepoStatusOptions);
            if (status.Untracked.Any())
            ***REMOVED***
                var untrackedFilesPatch = repo.Diff.Compare<Patch>(
                    status.Untracked.Select(u => u.FilePath), true, new ExplicitPathsOptions());
                ret.AddRange(GetGitFilePatch(untrackedFilesPatch));
    ***REMOVED***

            return ret;
***REMOVED***

        private IEnumerable<GitFilePatch> GetGitFilePatch(Patch filesPatch)
        ***REMOVED***
            var ret = new List<GitFilePatch>();
            using IEnumerator<PatchEntryChanges> patches = filesPatch.GetEnumerator();
            while (patches.MoveNext()
                   && patches.Current != null)
            ***REMOVED***
                ret.Add(new GitFilePatch
                ***REMOVED***
                    DiffContent = patches.Current.Patch,
                    AbsoluteLinesAdded = patches.Current.LinesAdded,
                    AbsoluteLinesDeleted = patches.Current.LinesDeleted,
                    FilePath = patches.Current.Path,
                    ChangeType = patches.Current.Status
        ***REMOVED***);
    ***REMOVED***

            return ret;
***REMOVED***
***REMOVED***
***REMOVED***
