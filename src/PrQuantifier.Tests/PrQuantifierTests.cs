namespace PrQuantifier.Tests
***REMOVED***
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Git;
    using global::PrQuantifier.Tests.Helpers;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class PrQuantifierTests : IDisposable
    ***REMOVED***
        private readonly Context context;
        private readonly GitRepoTestHelpers gitRepoHelpers = new GitRepoTestHelpers();
        private readonly GitEngine gitEngine = new GitEngine();

        public PrQuantifierTests()
        ***REMOVED***
            // Setup QuantifierOptions for all tests
            context = ContextFactory.Load(@"Data\ContextModel3.txt");
            gitRepoHelpers.CreateRepo();
***REMOVED***

        [Fact]
        public async Task Quantify_NoChanges_ReturnsZeroCounts()
        ***REMOVED***
            // Arrange
            var prQuantifier = new PrQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(0, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
***REMOVED***

        [Fact]
        public async Task Quantify_UntrackedFilesOnly()
        ***REMOVED***
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            var prQuantifier = new PrQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(2, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
***REMOVED***

        [Fact]
        public async Task Quantify_ChangedTrackedFiles()
        ***REMOVED***
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 2);
            var prQuantifier = new PrQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(3, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(2, quantifierResult.QuantifiedLinesDeleted);
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
            gitRepoHelpers.DeleteRepoDirectory();
***REMOVED***
***REMOVED***
***REMOVED***
