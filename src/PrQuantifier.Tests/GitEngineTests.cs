namespace PrQuantifier.Tests
***REMOVED***
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using global::PrQuantifier.Core.Git;
    using global::PrQuantifier.Tests.Helpers;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class GitEngineTests
    ***REMOVED***
        private readonly GitRepoTestHelpers gitRepoHelpers = new GitRepoTestHelpers();

        public GitEngineTests()
        ***REMOVED***
            gitRepoHelpers.CreateRepo();
***REMOVED***

        [Fact]
        public void GetGitChanges_NoChangesNoExceptions()
        ***REMOVED***
            // Setup
            var gitEngine = new GitEngine();

            // Act, Assert
            var exception = Record.Exception(() => gitEngine.GetGitChanges(Environment.CurrentDirectory));
            Assert.Null(exception);
***REMOVED***

        [Fact]
        public void Quantify_ChangedTrackedFiles()
        ***REMOVED***
            // Arrange
            var gitEngine = new GitEngine();
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
***REMOVED***
***REMOVED***
***REMOVED***
