namespace PullRequestQuantifier.Common.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using System.Linq;
    using LibGit2Sharp;
    using PullRequestQuantifier.Common.Extensions;

    [ExcludeFromCodeCoverage]
    public sealed class GitRepoTestHelpers
    {
        private IFileSystem fileSystem;

        public string RepoPath { get; private set; }

        public void CreateRepo()
        {
            // Setup test directory for git
            fileSystem = new FileSystem();
            var tempPath = fileSystem.Path.GetTempPath();
            RepoPath = fileSystem.Path.Combine(tempPath, $"fakeRepo-{Guid.NewGuid()}");
            if (fileSystem.Directory.Exists(RepoPath))
            {
                fileSystem.Directory.Delete(RepoPath, true);
            }

            fileSystem.Directory.CreateDirectory(RepoPath);

            // Init test git repository with gitignore
            Repository.Init(RepoPath);
            var gitIgnoreContent = fileSystem.File.ReadAllText(@"Data/TestGitIgnore.txt");
            fileSystem.File.WriteAllText(fileSystem.Path.Combine(RepoPath, ".gitignore"), gitIgnoreContent);
            CommitFilesToRepo();
        }

        public Commit CommitFilesToRepo()
        {
            var repo = new Repository(RepoPath);
            Commands.Stage(repo, "*");
            var author = new Signature("FakeUser", "fakeemail", DateTimeOffset.Now);
            return repo.Commit("Adding files", author, author);
        }

        public void AddUntrackedFileToRepoWithContent(string relativePath, string content)
        {
            fileSystem.File.WriteAllText(fileSystem.Path.Combine(RepoPath, relativePath), content);
        }

        public void AddUntrackedFileToRepoWithNumLines(string relativePath, int numLines)
        {
            string lineContent = $"Fake content line.{Environment.NewLine}";
            fileSystem.File.WriteAllText(fileSystem.Path.Combine(RepoPath, relativePath), string.Concat(Enumerable.Repeat(lineContent, numLines)));
        }

        public void RenameFile(string relativeFilePath, string renameToRelativeFilePath)
        {
            fileSystem.File.Move(
                fileSystem.Path.Combine(RepoPath, relativeFilePath),
                fileSystem.Path.Combine(RepoPath, renameToRelativeFilePath));
        }

        // This implementation of delete directory is based on the stack overflow
        // answer https://stackoverflow.com/questions/1701457/directory-delete-doesnt-work-access-denied-error-but-under-windows-explorer-it.
        // Otherwise this runs into access issues during direct deletion sometimes.
        public void DeleteRepoDirectory()
        {
            fileSystem.DeleteDirectory(RepoPath);
        }
    }
}
