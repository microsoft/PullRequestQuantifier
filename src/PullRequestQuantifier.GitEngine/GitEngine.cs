namespace PullRequestQuantifier.GitEngine
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
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
            IncludeIgnored = false,
            DetectRenamesInWorkDir = true,
            DetectRenamesInIndex = true
        };

        /// <inheritdoc />
        public IEnumerable<GitFilePatch> GetGitChange(
            string path,
            string commitSha1)
        {
            var ret = new ConcurrentBag<GitFilePatch>();
            var repoRoot = Repository.Discover(path);

            // don't crash when there is no repo to this path, return empty changes
            if (repoRoot == null)
            {
                return ret;
            }

            using var repo = new Repository(repoRoot);

            try
            {
                var commits = repo.Commits.QueryBy(new CommitFilter
                {
                    IncludeReachableFrom = commitSha1
                });

                Parallel.ForEach(commits, commit =>
                {
                    if (!commit.Sha.Equals(commitSha1, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return;
                    }

                    foreach (var parent in commit.Parents)
                    {
                        var patch = repo.Diff.Compare<Patch>(parent.Tree, commit.Tree);

                        foreach (var gitFilePatch in GetGitFilePatch(patch))
                        {
                            ret.Add(gitFilePatch);
                        }
                    }
                });
            }
            catch (NotFoundException)
            {
            }

            return ret;
        }

        /// <inheritdoc />
        public IEnumerable<GitFilePatch> GetGitChanges(string path)
        {
            var ret = new List<GitFilePatch>();
            var repoRoot = Repository.Discover(path);
            using var repo = new Repository(repoRoot);
            var status = repo.RetrieveStatus(RepoStatusOptions);

            var trackedFilesPatch = repo.Diff.Compare<Patch>();
            ret.AddRange(GetGitFilePatch(trackedFilesPatch, status));

            if (status.Untracked.Any())
            {
                var untrackedFilesPatch = repo.Diff.Compare<Patch>(
                    status.Untracked.Select(u => u.FilePath),
                    true,
                    new ExplicitPathsOptions { ShouldFailOnUnmatchedPath = false },
                    new CompareOptions { Similarity = SimilarityOptions.Exact });
                ret.AddRange(GetGitFilePatch(untrackedFilesPatch, status));
            }

            // renamed files (the addition part) is not part of tracked/untracked section
            // exclude from here already tracked files
            if (status.Modified.Any())
            {
                var files = status.Modified.Select(u => u.FilePath).Except(ret.Select(r => r.FilePath)).ToArray();
                if (files.Length > 0)
                {
                    var modifiedFilesPatch = repo.Diff.Compare<Patch>(
                        files,
                        true,
                        new ExplicitPathsOptions { ShouldFailOnUnmatchedPath = false },
                        new CompareOptions { Similarity = SimilarityOptions.Exact });
                    ret.AddRange(GetGitFilePatch(modifiedFilesPatch, status));
                }
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

            // don't crash when there is no repo to this path, return empty changes
            if (repoRoot == null)
            {
                return ret;
            }

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
        public FileSystem CreateRepository(string repositoryPath)
        {
            var fileSystem = new FileSystem();
            if (fileSystem.Directory.Exists(repositoryPath))
            {
                fileSystem.Directory.Delete(repositoryPath, true);
            }

            Repository.Init(repositoryPath);

            return fileSystem;
        }

        /// <inheritdoc />
        public string GetRepoRoot(string path)
        {
            return Repository.Discover(path);
        }

        private IEnumerable<GitFilePatch> GetGitFilePatch(
            Patch filesPatch,
            RepositoryStatus status = null)
        {
            var ret = new List<GitFilePatch>();
            using IEnumerator<PatchEntryChanges> patches = filesPatch.GetEnumerator();
            while (patches.MoveNext()
                   && patches.Current != null)
            {
                // minimum 88 score threshold was chose based on observation, if similarity will be less
                // than 88 then the deleted file will be marked as deleted and the other one as addition
                // if the file is part of the rename set the change type explicitly to rename,
                // otherwise git will set it to added or deleted
                ChangeKind changeKind = status?.RenamedInWorkDir.Count(r => r.IndexToWorkDirRenameDetails != null &&
                                                                            (r.IndexToWorkDirRenameDetails.NewFilePath
                                                                                 .Equals(patches.Current.Path)
                                                                             || r.IndexToWorkDirRenameDetails
                                                                                 .OldFilePath
                                                                                 .Equals(patches.Current.Path))
                                                                            && r.IndexToWorkDirRenameDetails
                                                                                .Similarity >= 88) > 0
                    ? ChangeKind.Renamed
                    : patches.Current.Status;

                ret.Add(new GitFilePatch(
                    patches.Current.Path,
                    new FileInfo(patches.Current.Path).Extension)
                {
                    DiffContent = patches.Current.Patch,
                    AbsoluteLinesAdded = patches.Current.LinesAdded,
                    AbsoluteLinesDeleted = patches.Current.LinesDeleted,
                    ChangeType = changeKind.ConvertToChangeType()
                });
            }

            return ret;
        }
    }
}
