namespace PullRequestQuantifier.GitEngine
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using LibGit2Sharp;
    using PullRequestQuantifier.Abstractions.Git;

    public sealed class GitEngine : IGitEngine
    {
        private static readonly StatusOptions RepoStatusOptions = new StatusOptions
        {
            IncludeUntracked = true,
            RecurseUntrackedDirs = true,
            IncludeIgnored = false
        };

        /// <inheritdoc />
        public IEnumerable<GitFilePatch> GetGitChanges(string path)
        {
            var ret = new List<GitFilePatch>();
            var repoRoot = Repository.Discover(path);
            using var repo = new Repository(repoRoot);

            var trackedFilesPatch = repo.Diff.Compare<Patch>();
            ret.AddRange(GetGitFilePatch(trackedFilesPatch));

            var status = repo.RetrieveStatus(RepoStatusOptions);
            if (status.Untracked.Any())
            {
                var untrackedFilesPatch = repo.Diff.Compare<Patch>(
                    status.Untracked.Select(u => u.FilePath),
                    true,
                    new ExplicitPathsOptions { ShouldFailOnUnmatchedPath = false });
                ret.AddRange(GetGitFilePatch(untrackedFilesPatch));
            }

            return ret;
        }

        /// <inheritdoc />
        public IEnumerable<Commit> GetAllCommits(string path)
        {
            var ret = new List<Commit>();
            var repoRoot = Repository.Discover(path);
            using var repo = new Repository(repoRoot);

            ret.AddRange(repo.Commits);

            return ret;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> GetGitHistoricalChangesToParent(string path)
        {
            var ret = new ConcurrentDictionary<GitCommit, IEnumerable<GitFilePatch>>();
            var repoRoot = Repository.Discover(path);
            using var repo = new Repository(repoRoot);

            var commits = repo.Commits.QueryBy(new CommitFilter
            {
                SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Time
            });
            Parallel.ForEach(commits, commit =>
            {
                var gotCommit = new GitCommit
                {
                    AuthorName = commit.Author.Name,
                    DateTimeOffset = commit.Author.When,
                    Sha = commit.Sha,
                    Title = commit.MessageShort
                };

                var changes = new List<GitFilePatch>();
                ret[gotCommit] = changes;

                foreach (var parent in commit.Parents)
                {
                    var patch = repo.Diff.Compare<Patch>(parent.Tree, commit.Tree);
                    changes.AddRange(GetGitFilePatch(patch));
                }
            });

            return ret;
        }

        /// <inheritdoc />
        public string GetRepoRoot(string path)
        {
            return Repository.Discover(path);
        }

        private IEnumerable<GitFilePatch> GetGitFilePatch(Patch filesPatch)
        {
            var ret = new List<GitFilePatch>();
            using IEnumerator<PatchEntryChanges> patches = filesPatch.GetEnumerator();
            while (patches.MoveNext()
                   && patches.Current != null)
            {
                ret.Add(new GitFilePatch
                {
                    DiffContent = patches.Current.Patch,
                    DiffContentLines = patches.Current.Patch.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                        .Select(l => l.Trim()).ToArray(),
                    AbsoluteLinesAdded = patches.Current.LinesAdded,
                    AbsoluteLinesDeleted = patches.Current.LinesDeleted,
                    FilePath = patches.Current.Path,
                    FileExtension = new FileInfo(patches.Current.Path).Extension,
                    ChangeType = patches.Current.Status.ConvertToChangeType()
                });
            }

            return ret;
        }
    }
}
