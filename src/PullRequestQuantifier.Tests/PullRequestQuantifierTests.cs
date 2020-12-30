namespace PullRequestQuantifier.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;
    using global::PullRequestQuantifier.Common.Tests;
    using global::PullRequestQuantifier.GitEngine;
    using Xunit;

    // todo add  more tests using different contexts
    [ExcludeFromCodeCoverage]
    public sealed class PullRequestQuantifierTests : IDisposable
    {
        private readonly Context context;
        private readonly GitRepoTestHelpers gitRepoHelpers = new ();
        private readonly GitEngine gitEngine = new ();

        public PullRequestQuantifierTests()
        {
            // Setup QuantifierOptions for all tests
            context = ContextFactory.Load(@"Data\ContextModel3.yaml");
            gitRepoHelpers.CreateRepo();
        }

        [Fact]
        public async Task Quantify_NoChanges_ReturnsZeroCounts()
        {
            // Arrange
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(0, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_UntrackedFilesOnly()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(2, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_WithIncludedFilterOnly()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake.csproj", 2);
            context.Included = new[] { "*.cs" };
            context.Excluded = new string[] { };
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(4, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_WithExcludedFilterOnly()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake.xml", 2);
            context.Included = new string[] { };
            context.Excluded = new[] { "*.xml" };
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(4, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_WithBothIncludedAndExcludedFilters()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake.xml", 2);
            context.Included = new[] { "*.xml" };
            context.Excluded = new[] { "*.xml" };
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(2, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_ChangedTrackedFiles()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepo("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepo("fake2.cs", 2);
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(3, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(2, quantifierResult.QuantifiedLinesDeleted);
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
            gitRepoHelpers.DeleteRepoDirectory();
        }
    }
}
