namespace PullRequestQuantifier.Client.Tests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using PullRequestQuantifier.Abstractions.Core;
    using PullRequestQuantifier.Client.Extensions;
    using PullRequestQuantifier.Client.QuantifyClient;
    using PullRequestQuantifier.Common.Tests;
    using PullRequestQuantifier.GitEngine;
    using Xunit;

    public sealed class QuantifierResultExtensionsTests
    {
        private const string RepositoryLink = "http://www.test.com";
        private const string ContextFileLink = "http://www.test1.com";
        private const string PullRequestLink = "http://www.test2.com";
        private const string AuthorName = "name";
        private static string assertCommentSummaryCollapsed;
        private readonly GitRepoTestHelpers gitRepoHelpers = new GitRepoTestHelpers();
        private readonly GitEngine gitEngine = new GitEngine();

        public QuantifierResultExtensionsTests()
        {
            gitRepoHelpers.CreateRepo();
            assertCommentSummaryCollapsed = File.ReadAllText(@"Data\AssertCommentSummaryCollapsed.txt");
        }

        [Fact]
        public async Task ToMarkdownCommentAsync_Successful()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);
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
        [InlineData(true)]
        [InlineData(false)]
        public async Task ToMarkdownCommentAsync_Options_CollapseChangesSummarySection(
            bool collapseChangesSummarySection)
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);
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
                new MarkdownCommentOptions
                {
                    CollapseChangesSummarySection = collapseChangesSummarySection
                });

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(comment));
            Assert.StartsWith("### Pull Request Quantified", comment);
            if (collapseChangesSummarySection)
            {
                Assert.True(comment.IndexOf(
                    await File.ReadAllTextAsync(@"Data\AssertCommentSummaryCollapsed.txt"),
                    StringComparison.Ordinal) > -1);
            }
            else
            {
                Assert.True(comment.IndexOf(
                    await File.ReadAllTextAsync(@"Data\AssertCommentSummaryNotCollapsed.txt"),
                    StringComparison.Ordinal) > -1);
            }
        }
    }
}
