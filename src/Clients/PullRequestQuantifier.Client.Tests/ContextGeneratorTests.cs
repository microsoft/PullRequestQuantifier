namespace PullRequestQuantifier.Client.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using PullRequestQuantifier.Client.ContextGenerator;
    using PullRequestQuantifier.Common.Tests;
    using Xunit;

    public sealed class ContextGeneratorTests : IDisposable
    {
        private readonly GitRepoTestHelpers gitRepoHelpers = new GitRepoTestHelpers();

        public ContextGeneratorTests()
        {
            gitRepoHelpers.CreateRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake12.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);
        }

        [Fact]
        public async Task CreateContextTest()
        {
            // setup
            var contextGenerator = new ContextGenerator();

            // act
            var context = await contextGenerator.Create(gitRepoHelpers.RepoPath);

            // assert
            Assert.True(context.Thresholds.Count() == 5);
            Assert.True(context.AdditionPercentile.Count == 2);
            Assert.True(context.DeletionPercentile.Count == 1);
        }

        [Fact]
        public async Task CreateTestRepoEmpty()
        {
            // setup
            var contextGenerator = new ContextGenerator();

            // act
            var context = await contextGenerator.Create(string.Empty);

            // assert
            Assert.True(context.Thresholds.Count() == 5);
        }

        public void Dispose()
        {
            gitRepoHelpers.DeleteRepoDirectory();
        }
    }
}
