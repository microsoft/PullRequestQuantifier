namespace PrQuantifier.Tests
***REMOVED***
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Abstractions.Context;
    using Abstractions.Core;
    using global::PrQuantifier.GitEngine;
    using global::PrQuantifier.Common.Tests;
    using Xunit;

    // todo add  more tests using different contexts
    [ExcludeFromCodeCoverage]
    public sealed class PrQuantifierTests : IDisposable
    ***REMOVED***
        private readonly Context context;
        private readonly GitRepoTestHelpers gitRepoHelpers = new ();
        private readonly GitEngine gitEngine = new ();

        public PrQuantifierTests()
        ***REMOVED***
            // Setup QuantifierOptions for all tests
            context = ContextFactory.Load(@"Data\ContextModel3.yaml");
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
        public async Task Quantify_WithIncludedFilterOnly()
        ***REMOVED***
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake.csproj", 2);
            context.Included = new[] ***REMOVED*** "*.cs" ***REMOVED***;
            context.Excluded = new string[] ***REMOVED*** ***REMOVED***;
            var prQuantifier = new PrQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(4, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
***REMOVED***

        [Fact]
        public async Task Quantify_WithExcludedFilterOnly()
        ***REMOVED***
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake.xml", 2);
            context.Included = new string[] ***REMOVED*** ***REMOVED***;
            context.Excluded = new[] ***REMOVED*** "*.xml" ***REMOVED***;
            var prQuantifier = new PrQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(4, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
***REMOVED***

        [Fact]
        public async Task Quantify_WithBothIncludedAndExcludedFilters()
        ***REMOVED***
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake.xml", 2);
            context.Included = new[] ***REMOVED*** "*.xml" ***REMOVED***;
            context.Excluded = new[] ***REMOVED*** "*.xml" ***REMOVED***;
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
