namespace PullRequestQuantifier.Common.Tests
***REMOVED***
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using LibGit2Sharp;

    [ExcludeFromCodeCoverage]
    public sealed class GitRepoTestHelpers
    ***REMOVED***
        private IFileSystem fileSystem;

        public string RepoPath ***REMOVED*** get; private set; ***REMOVED***

        public void CreateRepo()
        ***REMOVED***
            // Setup test directory for git
            fileSystem = new FileSystem();
            var tempPath = fileSystem.Path.GetTempPath();
            RepoPath = fileSystem.Path.Combine(tempPath, $"fakeRepo-***REMOVED***Guid.NewGuid()***REMOVED***");
            if (fileSystem.Directory.Exists(RepoPath))
            ***REMOVED***
                fileSystem.Directory.Delete(RepoPath, true);
    ***REMOVED***

            fileSystem.Directory.CreateDirectory(RepoPath);

            // Init test git repository with gitignore
            Repository.Init(RepoPath);
            var gitIgnoreContent = fileSystem.File.ReadAllText(@"Data\TestGitIgnore.txt");
            fileSystem.File.WriteAllText(fileSystem.Path.Combine(RepoPath, ".gitignore"), gitIgnoreContent);
            CommitFilesToRepo();
***REMOVED***

        public void CommitFilesToRepo()
        ***REMOVED***
            var repo = new Repository(RepoPath);
            Commands.Stage(repo, "*");
            var author = new Signature("FakeUser", "fakeemail", DateTimeOffset.Now);
            repo.Commit("Adding files", author, author);
***REMOVED***

        public void AddUntrackedFileToRepo(string relativePath, int numLines)
        ***REMOVED***
            string lineContent = $"Fake content line.***REMOVED***Environment.NewLine***REMOVED***";
            fileSystem.File.WriteAllText(fileSystem.Path.Combine(RepoPath, relativePath), string.Concat(Enumerable.Repeat(lineContent, numLines)));
***REMOVED***

        // This implementation of delete directory is based on the stack overflow
        // answer https://stackoverflow.com/questions/1701457/directory-delete-doesnt-work-access-denied-error-but-under-windows-explorer-it.
        // Otherwise this runs into access issues during direct deletion sometimes.
        public void DeleteRepoDirectory()
        ***REMOVED***
            var dirInfo = fileSystem.DirectoryInfo.FromDirectoryName(RepoPath);
            if (dirInfo.Exists)
            ***REMOVED***
                try
                ***REMOVED***
                    SetNormalAttribute(dirInfo);
                    dirInfo.Delete(true);
        ***REMOVED***
#pragma warning disable CA1031 // Do not catch general exception types
                catch
#pragma warning restore CA1031 // Do not catch general exception types
                ***REMOVED***
        ***REMOVED***
    ***REMOVED***
***REMOVED***

        private static void SetNormalAttribute(IDirectoryInfo dirInfo)
        ***REMOVED***
            if (!dirInfo.Exists)
            ***REMOVED***
                return;
    ***REMOVED***

            var directories = dirInfo.GetDirectories().ToArray();
            foreach (var subDir in directories)
            ***REMOVED***
                SetNormalAttribute(subDir);
    ***REMOVED***

            var files = dirInfo.GetFiles().ToArray();
            foreach (var file in files)
            ***REMOVED***
                file.Attributes = FileAttributes.Normal;
    ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
