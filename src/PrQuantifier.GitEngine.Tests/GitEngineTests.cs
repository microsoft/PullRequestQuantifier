namespace PrQuantifier.GitEngine.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using PrQuantifier.Common.Tests;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class GitEngineTests
    {
        private readonly GitRepoTestHelpers gitRepoHelpers = new GitRepoTestHelpers();

        public GitEngineTests()
        {
            gitRepoHelpers.CreateRepo();
        }

        [Fact]
        public void GetGitChanges_NoChangesNoExceptions()
        {
            // Setup
            IGitEngine gitEngine = new GitEngine();

            // Act, Assert
            var exception = Record.Exception(() => gitEngine.GetGitChanges(Environment.CurrentDirectory));
            Assert.Null(exception);
        }

        [Fact]
        public void GetGitChanges_ChangedTrackedFiles()
        {
            // Arrange
            IGitEngine gitEngine = new GitEngine();
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 2);

            // Act
            var gitChanges = gitEngine.GetGitChanges(gitRepoHelpers.RepoPath).ToArray();

            // Assert
            Assert.NotEmpty(gitChanges);
            Assert.Equal(2, gitChanges.Length);
            Assert.Equal(3, gitChanges[0].AbsoluteLinesAdded);
            Assert.Equal(2, gitChanges[1].AbsoluteLinesDeleted);
        }

        [Fact]
        public void GetAllCommits_Successful()
        {
            // Arrange
            IGitEngine gitEngine = new GitEngine();
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 2);

            // Act
            var commits = gitEngine.GetAllCommits(gitRepoHelpers.RepoPath).ToArray();

            // Assert
            Assert.NotEmpty(commits);
            Assert.Equal(2, commits.Length);
        }

        [Fact]
        public void GetGitHistoricalChangesToParent_Successful()
        {
            // Arrange
            IGitEngine gitEngine = new GitEngine();
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 2);

            // Act
            var commits = gitEngine.GetGitHistoricalChangesToParent(gitRepoHelpers.RepoPath).ToArray();

            // Assert
            Assert.NotEmpty(commits);
            Assert.Equal(2, commits.Length);
        }
    }
}
