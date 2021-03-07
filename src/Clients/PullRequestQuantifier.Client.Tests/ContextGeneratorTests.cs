namespace PullRequestQuantifier.Client.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using PullRequestQuantifier.Client.ContextGenerator;
    using PullRequestQuantifier.Common.Tests;
    using Xunit;

    public sealed class ContextGeneratorTests
    {
        [Fact]
        public async Task CreateTestRepo()
        {
            // setup
            var gitRepoHelpers = new GitRepoTestHelpers();
            gitRepoHelpers.CreateRepo();
            var contextGenerator = new ContextGenerator();

            // act
            var context = await contextGenerator.Create(gitRepoHelpers.RepoPath);

            // assert
            Assert.True(context.Thresholds.Count() == 5);
            Assert.True(context.AdditionPercentile.Count == 1);
            Assert.True(context.DeletionPercentile.Count == 1);

            // cleanup
            gitRepoHelpers.DeleteRepoDirectory();
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
            Assert.True(context.AdditionPercentile.Count == 66);
            Assert.True(context.DeletionPercentile.Count == 41);
        }
    }
}
