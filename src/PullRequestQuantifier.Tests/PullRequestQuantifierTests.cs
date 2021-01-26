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
        private readonly GitRepoTestHelpers gitRepoHelpers = new GitRepoTestHelpers();
        private readonly GitEngine gitEngine = new GitEngine();

        public PullRequestQuantifierTests()
        {
            // Setup QuantifierOptions for all tests
            context = ContextFactory.Load(@"Data/ContextModel3.yaml");
            context.LanguageOptions = new LanguageOptions();
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
            Assert.Equal("No Changes", quantifierResult.Label);
            Assert.Equal(0, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_UntrackedFilesOnly()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(!string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(2, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_FilesWithPlusMinusContent()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithContent("fake.cs", "-text\n+add\nnormal\n-44");
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(!string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(4, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_IgnoreSpacesFalse()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithContent("fake.cs", "some text\n\nmore text");
            context.LanguageOptions.IgnoreSpaces = false;
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(!string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(3, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_IgnoreSpacesTrue()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithContent("fake.cs", "some text\n\nmore text");
            context.LanguageOptions.IgnoreSpaces = true;
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(!string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(2, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_IgnoreCommentsTrue()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithContent("fake.cs", "some text\n// comment text\n normal text");
            context.LanguageOptions.IgnoreComments = true;
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(!string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(2, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_WithIncludedFilterOnly()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.csproj", 2);
            context.Included = new[] { "*.cs" };
            context.Excluded = new string[] { };
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(!string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(4, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_WithExcludedFilterOnly()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.xml", 2);
            context.Included = new string[] { };
            context.Excluded = new[] { "*.xml" };
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(!string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(4, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_WithBothIncludedAndExcludedFilters()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.xml", 2);
            context.Included = new[] { "*.xml" };
            context.Excluded = new[] { "*.xml" };
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(!string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(2, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(0, quantifierResult.QuantifiedLinesDeleted);
        }

        [Fact]
        public async Task Quantify_ChangedTrackedFiles()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);
            var prQuantifier = new PullRequestQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));

            // Act
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // Assert
            Assert.True(!string.IsNullOrEmpty(quantifierResult.Label));
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