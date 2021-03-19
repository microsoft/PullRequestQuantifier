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
    public sealed class QuantifierResultExtensionsTests : IDisposable
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
            Assert.StartsWith("### ![](https://img.shields.io/static/v1?label=Quantified&message=Extra%20Small&color=green)", comment);
        }

        [Fact]
        public async Task FilesWithoutExt_ShowFullPathInSummary()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2", 2);
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
            Assert.True(comment.IndexOf("fake2 : +0 -2", StringComparison.Ordinal) > -1);
            Assert.True(comment.IndexOf(".cs : +3 -0", StringComparison.Ordinal) > -1);
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
            Assert.StartsWith("### ![](https://img.shields.io/static/v1?label=Quantified&message=Extra%20Small&color=green)", comment);
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

        public void Dispose()
        {
            gitRepoHelpers.DeleteRepoDirectory();
        }
    }
}
