namespace PullRequestQuantifier.Client.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using PullRequestQuantifier.Client.ContextGenerator;
    using Xunit;

    [ExcludeFromCodeCoverage]
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
            Assert.NotEmpty(context.FormulaPercentile);
        }
    }
}
