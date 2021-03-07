namespace PullRequestQuantifier.Client.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using PullRequestQuantifier.Client.ContextGenerator;
    using Xunit;

    public sealed class ContextGeneratorTests
    {
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
    }
}
