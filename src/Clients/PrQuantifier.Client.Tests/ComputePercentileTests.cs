namespace PrQuantifier.Client.Tests
***REMOVED***
    using System;
    using Xunit;

    public sealed class ComputePercentileTests
    ***REMOVED***
        [Theory]
        [InlineData(new[] ***REMOVED*** 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 ***REMOVED***, 92, 100)]
        [InlineData(new[] ***REMOVED*** 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 ***REMOVED***, 5, 10)]
        [InlineData(new[] ***REMOVED*** 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 ***REMOVED***, 50, 60)]
        [InlineData(new[] ***REMOVED*** 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 ***REMOVED***, 0, 0)]
        [InlineData(new[] ***REMOVED*** 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 ***REMOVED***, 120, 100)]
        public void Percentile_Successful(
            int[] data,
            int value,
            int expectation)
        ***REMOVED***
            // Setup
            Array.Sort(data);

            // Act, Assert
            Assert.Equal(expectation, Math.Round(ComputePercentile.Percentile(data, value), 0));
***REMOVED***
***REMOVED***
***REMOVED***
