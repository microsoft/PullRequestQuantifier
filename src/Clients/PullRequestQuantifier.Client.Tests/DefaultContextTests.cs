namespace PullRequestQuantifier.Client.Tests
***REMOVED***
    using PullRequestQuantifier.Client.ContextGenerator;
    using Xunit;

    public sealed class DefaultContextTests
    ***REMOVED***
         [Fact]
         public void DefaultContext_Successful()
        ***REMOVED***
            // Set up
            var context = DefaultContext.Value;

            // Act, Assert
            Assert.NotEmpty(context.AdditionPercentile);
            Assert.NotEmpty(context.DeletionPercentile);
***REMOVED***
***REMOVED***
***REMOVED***
