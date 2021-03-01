namespace PullRequestQuantifier.GitEngine.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using PullRequestQuantifier.Abstractions.Git;
    using PullRequestQuantifier.Common.Tests;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class GitEngineTests : IDisposable
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
        public void GetGitChanges_ChangedModifiedRenamedFiles()
        {
            // Arrange
            IGitEngine gitEngine = new GitEngine();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fakeRename.cs", 2);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.RenameFile("fakeRename.cs", "fakeRename1.cs");

            // Act
            var gitChanges = gitEngine.GetGitChanges(gitRepoHelpers.RepoPath).ToArray();

            // Assert
            Assert.Single(gitChanges);
            Assert.Equal(2, gitChanges[0].AbsoluteLinesDeleted);
            Assert.Equal(GitChangeType.Renamed, gitChanges[0].ChangeType);
        }

        [Fact]
        public void GetGitChanges_ChangedTrackedFiles()
        {
            // Arrange
            IGitEngine gitEngine = new GitEngine();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);

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
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);

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
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);

            // Act
            var commits = gitEngine.GetGitHistoricalChangesToParent(gitRepoHelpers.RepoPath).ToArray();

            // Assert
            Assert.NotEmpty(commits);
            Assert.Equal(2, commits.Length);
        }

        [Fact]
        public void GetGitChange_OneChange_Successful()
        {
            // Arrange
            IGitEngine gitEngine = new GitEngine();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            var commit = gitRepoHelpers.CommitFilesToRepo();

            // Act
            var changes = gitEngine.GetGitChange(
                gitRepoHelpers.RepoPath,
                commit.Sha).ToArray();

            // Assert
            Assert.NotEmpty(changes);
            Assert.Equal(2, changes.Length);
        }

        [Fact]
        public void GetGitChange_MoreChanges_Successful()
        {
            // Arrange
            IGitEngine gitEngine = new GitEngine();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake3.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake4.cs", 2);
            var commit = gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake6.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake5.cs", 2);
            gitRepoHelpers.CommitFilesToRepo();

            // Act
            var changes = gitEngine.GetGitChange(
                gitRepoHelpers.RepoPath,
                commit.Sha).ToArray();

            // Assert
            Assert.NotEmpty(changes);
            Assert.Equal(2, changes.Length);
            Assert.Contains(changes, c => c.FilePath.Equals("fake3.cs", StringComparison.InvariantCultureIgnoreCase));
            Assert.Contains(changes, c => c.FilePath.Equals("fake4.cs", StringComparison.InvariantCultureIgnoreCase));
        }

        public void Dispose()
        {
            gitRepoHelpers.DeleteRepoDirectory();
        }
    }
}
