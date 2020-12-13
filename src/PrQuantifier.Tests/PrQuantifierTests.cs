namespace PrQuantifier.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using global::PrQuantifier.Common.Tests;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Git;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class PrQuantifierTests : IDisposable
    {
        private readonly Context context;
        private readonly GitRepoTestHelpers gitRepoHelpers = new GitRepoTestHelpers();
        private readonly GitEngine gitEngine = new GitEngine();

        public PrQuantifierTests()
        {
            // Setup QuantifierOptions for all tests
            context = ContextFactory.Load(@"Data\ContextModel3.txt");
            gitRepoHelpers.CreateRepo();
        }

        [Fact]
        public async Task Quantify_NoChanges_ReturnsZeroCounts()
        {
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
        }

        [Fact]
        public async Task Quantify_UntrackedFilesOnly()
        {
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
            var prQuantifier = new PrQuantifier(context);
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
