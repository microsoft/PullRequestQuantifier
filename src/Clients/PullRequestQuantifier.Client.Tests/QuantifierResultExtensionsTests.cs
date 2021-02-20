namespace PullRequestQuantifier.Client.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;
    using PullRequestQuantifier.Abstractions.Core;
    using PullRequestQuantifier.Client.Extensions;
    using PullRequestQuantifier.Client.QuantifyClient;
    using PullRequestQuantifier.Common.Tests;
    using PullRequestQuantifier.GitEngine;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class QuantifierResultExtensionsTests
    {
        private const string RepositoryLink = "http://www.test.com";
        private const string ContextFileLink = "http://www.test1.com";
        private const string PullRequestLink = "http://www.test2.com";
        private const string AuthorName = "name";
        private readonly GitRepoTestHelpers gitRepoHelpers = new GitRepoTestHelpers();
        private readonly GitEngine gitEngine = new GitEngine();

        public QuantifierResultExtensionsTests()
        {
            gitRepoHelpers.CreateRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);
        }

        [Fact]
        public async Task ToMarkdownCommentAsync_Successful()
        {
            // Arrange
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));
            var quantifyClient = new QuantifyClient(string.Empty);

            // Act
            var quantifierResult = await quantifyClient.Compute(quantifierInput);
            var comment = await quantifierResult.ToMarkdownCommentAsync(
                RepositoryLink,
                ContextFileLink,
                PullRequestLink,
                AuthorName);

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(comment));
            Assert.StartsWith("### Pull Request Quantified", comment);
        }

        [Theory]
        [InlineData(33, false)]
        [InlineData(0, false)]
        [InlineData(80, true)]
        [InlineData(82, true)]
        [InlineData(182, true)]
        [InlineData(400, true)]
        [InlineData(410, false)]
        [InlineData(1410, false)]
        [InlineData(10410, false)]
        public async Task ToMarkdownCommentAsync_IdealSizeCheck(int formulaLinesChanged, bool isIdeal)
        {
            // Arrange
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));
            var quantifyClient = new QuantifyClient(string.Empty);
            var quantifierResult = await quantifyClient.Compute(quantifierInput);
            quantifierResult.FormulaLinesChanged = formulaLinesChanged;

            var comment = await quantifierResult.ToMarkdownCommentAsync(
                RepositoryLink,
                ContextFileLink,
                PullRequestLink,
                AuthorName);

            // Assert
            var idealJobIndex = comment.IndexOf("Great job", StringComparison.Ordinal);
            Assert.True((isIdeal && idealJobIndex > -1) || (!isIdeal && idealJobIndex == -1));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ToMarkdownCommentAsync_Options_CollapseQuantificationDetailsSection(bool collapseQuantifiedDetailsSection)
        {
            // Arrange
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));
            var quantifyClient = new QuantifyClient(string.Empty);

            // Act
            var quantifierResult = await quantifyClient.Compute(quantifierInput);
            var comment = await quantifierResult.ToMarkdownCommentAsync(
                RepositoryLink,
                ContextFileLink,
                PullRequestLink,
                AuthorName,
                false,
                new MarkdownCommentOptions
                {
                    CollapsePullRequestQuantifiedSection = collapseQuantifiedDetailsSection
                });

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(comment));
            Assert.StartsWith("### Pull Request Quantified", comment);
            if (collapseQuantifiedDetailsSection)
            {
                Assert.True(
                    comment.IndexOf(
                        await File.ReadAllTextAsync(@"Data/AssertCommentSummaryCollapsed.txt"),
                        StringComparison.Ordinal) > -1);
            }
            else
            {
                Assert.True(
                    comment.IndexOf(
                        await File.ReadAllTextAsync(@"Data/AssertCommentSummaryNotCollapsed.txt"),
                        StringComparison.Ordinal) > -1);
            }
        }
    }
}
