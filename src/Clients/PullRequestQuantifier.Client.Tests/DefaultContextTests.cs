namespace PullRequestQuantifier.Client.Tests
{
    using PullRequestQuantifier.Client.ContextGenerator;
    using Xunit;

    public sealed class DefaultContextTests
    {
         [Fact]
         public void DefaultContext_Successful()
        {
            // Set up
            var context = DefaultContext.Value;

            // Act, Assert
            Assert.NotEmpty(context.AdditionPercentile);
            Assert.NotEmpty(context.DeletionPercentile);
        }
    }
}
