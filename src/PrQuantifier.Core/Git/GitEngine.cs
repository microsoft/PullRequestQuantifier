namespace PrQuantifier.Core.Git
***REMOVED***
    using System.Collections.Generic;
    using System.Linq;
    using LibGit2Sharp;
    using PrQuantifier.Core.Abstractions;
    using PrQuantifier.Core.Extensions;

    public sealed class GitEngine : IGitEngine
    ***REMOVED***
        private static readonly StatusOptions RepoStatusOptions = new StatusOptions
        ***REMOVED***
            IncludeUntracked = true,
            RecurseUntrackedDirs = true,
            IncludeIgnored = false
***REMOVED***;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IEnumerable<Commit> GetAllCommits(string path)
        ***REMOVED***
            var ret = new List<Commit>();
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
                    ChangeType = patches.Current.Status.ConvertToChangeType()
        ***REMOVED***);
    ***REMOVED***

            return ret;
***REMOVED***
***REMOVED***
***REMOVED***
