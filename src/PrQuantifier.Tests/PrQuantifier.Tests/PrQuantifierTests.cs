namespace PrQuantifier.Tests
***REMOVED***
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using LibGit2Sharp;
    using Xunit;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class PrQuantifierTests : IDisposable
    ***REMOVED***
        private readonly QuantifierOptions quantifierOptions;

        private readonly IFileSystem fileSystem;

        private readonly string repoPath;

        public PrQuantifierTests()
        ***REMOVED***
            // Setup QuantifierOptions for all tests
            var yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var optionsYaml = File.ReadAllText("QuantifierOptions.yml");
            quantifierOptions = yamlDeserializer.Deserialize<QuantifierOptions>(optionsYaml);

            // Setup test directory for git
            fileSystem = new FileSystem();
            var tempPath = fileSystem.Path.GetTempPath();
            repoPath = fileSystem.Path.Combine(tempPath, $"fakeRepo-***REMOVED***Guid.NewGuid()***REMOVED***");
            if (fileSystem.Directory.Exists(repoPath))
            ***REMOVED***
                fileSystem.Directory.Delete(repoPath, true);
    ***REMOVED***

            fileSystem.Directory.CreateDirectory(repoPath);

            // Init test git repository with gitignore
            Repository.Init(repoPath);
            var gitignoreContent = fileSystem.File.ReadAllText("TestGitIgnore.txt");
            fileSystem.File.WriteAllText(fileSystem.Path.Combine(repoPath, ".gitignore"), gitignoreContent);
            CommitFilesToRepo();
***REMOVED***

        [Fact]
        public void Quantify_NoChanges_ReturnsZeroCounts()
        ***REMOVED***
            // Arrange
            var prQuantifier = new PrQuantifier(quantifierOptions);

            // Act
            var quantifierResult = prQuantifier.Quantify(repoPath);

            // Assert
            Assert.Equal(0, quantifierResult.Category);
            Assert.Equal(0, quantifierResult.ChangeCounts[OperationType.Add]);
            Assert.Equal(0, quantifierResult.ChangeCounts[OperationType.Delete]);
***REMOVED***

        [Fact]
        public void Quantify_UntrackedFilesOnly()
        ***REMOVED***
            // Arrange
            AddUntrackedFileToRepo("fake.cs", 2);
            var prQuantifier = new PrQuantifier(quantifierOptions);

            // Act
            var quantifierResult = prQuantifier.Quantify(repoPath);

            // Assert
            Assert.Equal(0, quantifierResult.Category);
            Assert.Equal(2, quantifierResult.ChangeCounts[OperationType.Add]);
            Assert.Equal(0, quantifierResult.ChangeCounts[OperationType.Delete]);
***REMOVED***

        [Fact]
        public void Quantify_ChangedTrackedFiles()
        ***REMOVED***
            // Arrange
            AddUntrackedFileToRepo("fake.cs", 2);
            AddUntrackedFileToRepo("fake2.cs", 4);
            CommitFilesToRepo();
            AddUntrackedFileToRepo("fake.cs", 5);
            AddUntrackedFileToRepo("fake2.cs", 2);
            var prQuantifier = new PrQuantifier(quantifierOptions);

            // Act
            var quantifierResult = prQuantifier.Quantify(repoPath);

            // Assert
            Assert.Equal(0, quantifierResult.Category);
            Assert.Equal(3, quantifierResult.ChangeCounts[OperationType.Add]);
            Assert.Equal(2, quantifierResult.ChangeCounts[OperationType.Delete]);
***REMOVED***

        [Fact]
        public void QuantifyAgainstBranch_ReturnsCorrectResult()
        ***REMOVED***
            // TODO: Implement
***REMOVED***

        [Fact]
        public void QuantifyCommit_ReturnsCorrectResult()
        ***REMOVED***
            // TODO: Implement
***REMOVED***

        public void Dispose()
        ***REMOVED***
            DeleteRepoDirectory();
***REMOVED***

        private void CommitFilesToRepo()
        ***REMOVED***
            var repo = new Repository(repoPath);
            Commands.Stage(repo, "*");
            var author = new Signature("FakeUser", "fakeemail", DateTimeOffset.Now);
            repo.Commit("Adding files", author, author);
***REMOVED***

        private void AddUntrackedFileToRepo(string relativePath, int numLines)
        ***REMOVED***
            string lineContent = $"Fake content line.***REMOVED***Environment.NewLine***REMOVED***";
            fileSystem.File.WriteAllText(fileSystem.Path.Combine(repoPath, relativePath), string.Concat(Enumerable.Repeat(lineContent, numLines)));
***REMOVED***

        // This implementation of delete directory is based on the stack overflow
        // answer https://stackoverflow.com/questions/1701457/directory-delete-doesnt-work-access-denied-error-but-under-windows-explorer-it.
        // Otherwise this runs into access issues during direct deletion sometimes.
        private void DeleteRepoDirectory()
        ***REMOVED***
            var dirInfo = fileSystem.DirectoryInfo.FromDirectoryName(repoPath);
            if (dirInfo.Exists)
            ***REMOVED***
                SetNormalAttribute(dirInfo);
    ***REMOVED***

            dirInfo.Delete(true);
***REMOVED***

        private void SetNormalAttribute(IDirectoryInfo dirInfo)
        ***REMOVED***
            foreach (var subDir in dirInfo.GetDirectories())
            ***REMOVED***
                SetNormalAttribute(subDir);
    ***REMOVED***

            foreach (var file in dirInfo.GetFiles())
            ***REMOVED***
                file.Attributes = FileAttributes.Normal;
    ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
