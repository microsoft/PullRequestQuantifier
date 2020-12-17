namespace PrQuantifier.Core.Git
***REMOVED***
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
            var repoRoot = Repository.Discover(path);
            using var repo = new Repository(repoRoot);

            ret.AddRange(repo.Commits);

            return ret;
***REMOVED***

        /// <inheritdoc />
        public IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> GetGitHistoricalChangesToParent(string path)
        ***REMOVED***
            var ret = new ConcurrentDictionary<GitCommit, IEnumerable<GitFilePatch>>();
            var repoRoot = Repository.Discover(path);
            using var repo = new Repository(repoRoot);

            var commits = repo.Commits.QueryBy(new CommitFilter
            ***REMOVED***
                SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Time
    ***REMOVED***);
            Parallel.ForEach(commits, commit =>
            ***REMOVED***
                var gotCommit = new GitCommit
                ***REMOVED***
                    AuthorName = commit.Author.Name,
                    DateTimeOffset = commit.Author.When,
                    Sha = commit.Sha,
                    Title = commit.MessageShort
        ***REMOVED***;

                var changes = new List<GitFilePatch>();
                ret[gotCommit] = changes;

                foreach (var parent in commit.Parents)
                ***REMOVED***
                    var patch = repo.Diff.Compare<Patch>(parent.Tree, commit.Tree);
                    changes.AddRange(GetGitFilePatch(patch));
        ***REMOVED***
    ***REMOVED***);

            return ret;
***REMOVED***

        /// <inheritdoc />
        public string GetRepoRoot(string path)
        ***REMOVED***
            return Repository.Discover(path);
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
                    DiffContentLines = patches.Current.Patch.Split("\n", StringSplitOptions.RemoveEmptyEntries),
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
