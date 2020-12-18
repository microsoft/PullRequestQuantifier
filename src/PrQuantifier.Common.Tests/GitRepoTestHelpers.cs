namespace PrQuantifier.Common.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using LibGit2Sharp;

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
            var gitIgnoreContent = fileSystem.File.ReadAllText(@"Data\TestGitIgnore.txt");
            fileSystem.File.WriteAllText(fileSystem.Path.Combine(RepoPath, ".gitignore"), gitIgnoreContent);
            CommitFilesToRepo();
        }

        public void CommitFilesToRepo()
        {
            var repo = new Repository(RepoPath);
            Commands.Stage(repo, "*");
            var author = new Signature("FakeUser", "fakeemail", DateTimeOffset.Now);
            repo.Commit("Adding files", author, author);
        }

        public void AddUntrackedFileToRepo(string relativePath, int numLines)
        {
            string lineContent = $"Fake content line.{Environment.NewLine}";
            fileSystem.File.WriteAllText(fileSystem.Path.Combine(RepoPath, relativePath), string.Concat(Enumerable.Repeat(lineContent, numLines)));
        }

        // This implementation of delete directory is based on the stack overflow
        // answer https://stackoverflow.com/questions/1701457/directory-delete-doesnt-work-access-denied-error-but-under-windows-explorer-it.
        // Otherwise this runs into access issues during direct deletion sometimes.
        public void DeleteRepoDirectory()
        {
            var dirInfo = fileSystem.DirectoryInfo.FromDirectoryName(RepoPath);
            if (dirInfo.Exists)
            {
                SetNormalAttribute(dirInfo);
            }

            dirInfo.Delete(true);
        }

        private static void SetNormalAttribute(IDirectoryInfo dirInfo)
        {
            foreach (var subDir in dirInfo.GetDirectories())
            {
                SetNormalAttribute(subDir);
            }

            foreach (var file in dirInfo.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }
        }
    }
}
