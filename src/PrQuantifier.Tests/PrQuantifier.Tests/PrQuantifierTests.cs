namespace PrQuantifier.Tests
{
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using LibGit2Sharp;
    using Xunit;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class PrQuantifierTests : IDisposable
    {
        private readonly QuantifierOptions quantifierOptions;

        private readonly IFileSystem fileSystem;

        private readonly string repoPath;

        public PrQuantifierTests()
        {
            // Setup QuantifierOptions for all tests
            var yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var optionsYaml = File.ReadAllText("QuantifierOptions.yml");
            quantifierOptions = yamlDeserializer.Deserialize<QuantifierOptions>(optionsYaml);

            // Setup test directory for git
            fileSystem = new FileSystem();
            var tempPath = fileSystem.Path.GetTempPath();
            repoPath = fileSystem.Path.Combine(tempPath, $"fakeRepo-{Guid.NewGuid()}");
            if (fileSystem.Directory.Exists(repoPath))
            {
                fileSystem.Directory.Delete(repoPath, true);
            }

            fileSystem.Directory.CreateDirectory(repoPath);

            // Init test git repository with gitignore
            Repository.Init(repoPath);
            var gitignoreContent = fileSystem.File.ReadAllText("TestGitIgnore.txt");
            fileSystem.File.WriteAllText(fileSystem.Path.Combine(repoPath, ".gitignore"), gitignoreContent);
            CommitFilesToRepo();
        }

        [Fact]
        public void Quantify_NoChanges_ReturnsZeroCounts()
        {
            // Arrange
            var prQuantifier = new PrQuantifier(quantifierOptions);

            // Act
            var quantifierResult = prQuantifier.Quantify(repoPath);

            // Assert
            Assert.Equal(0, quantifierResult.Category);
            Assert.Equal(0, quantifierResult.ChangeCounts[OperationType.Add]);
            Assert.Equal(0, quantifierResult.ChangeCounts[OperationType.Delete]);
        }

        [Fact]
        public void Quantify_UntrackedFilesOnly()
        {
            // Arrange
            AddUntrackedFileToRepo("fake.cs", 2);
            var prQuantifier = new PrQuantifier(quantifierOptions);

            // Act
            var quantifierResult = prQuantifier.Quantify(repoPath);

            // Assert
            Assert.Equal(0, quantifierResult.Category);
            Assert.Equal(2, quantifierResult.ChangeCounts[OperationType.Add]);
            Assert.Equal(0, quantifierResult.ChangeCounts[OperationType.Delete]);
        }

        [Fact]
        public void Quantify_ChangedTrackedFiles()
        {
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
        }

        [Fact]
        public void QuantifyAgainstBranch_ReturnsCorrectResult()
        {
            // TODO: Implement
        }

        [Fact]
        public void QuantifyCommit_ReturnsCorrectResult()
        {
            // TODO: Implement
        }

        public void Dispose()
        {
            DeleteRepoDirectory();
        }

        private void CommitFilesToRepo()
        {
            var repo = new Repository(repoPath);
            Commands.Stage(repo, "*");
            var author = new Signature("FakeUser", "fakeemail", DateTimeOffset.Now);
            repo.Commit("Adding files", author, author);
        }

        private void AddUntrackedFileToRepo(string relativePath, int numLines)
        {
            string lineContent = $"Fake content line.{Environment.NewLine}";
            fileSystem.File.WriteAllText(fileSystem.Path.Combine(repoPath, relativePath), string.Concat(Enumerable.Repeat(lineContent, numLines)));
        }

        // This implementation of delete directory is based on the stack overflow
        // answer https://stackoverflow.com/questions/1701457/directory-delete-doesnt-work-access-denied-error-but-under-windows-explorer-it.
        // Otherwise this runs into access issues during direct deletion sometimes.
        private void DeleteRepoDirectory()
        {
            var dirInfo = fileSystem.DirectoryInfo.FromDirectoryName(repoPath);
            if (dirInfo.Exists)
            {
                SetNormalAttribute(dirInfo);
            }

            dirInfo.Delete(true);
        }

        private void SetNormalAttribute(IDirectoryInfo dirInfo)
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
