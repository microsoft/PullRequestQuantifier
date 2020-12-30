namespace PullRequestQuantifier.Client.Tests
{
    using System;
    using Xunit;

    public sealed class ComputePercentileTests
    {
        [Theory]
        [InlineData(new[] { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 92, 100)]
        [InlineData(new[] { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 5, 10)]
        [InlineData(new[] { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 50, 60)]
        [InlineData(new[] { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 0, 0)]
        [InlineData(new[] { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 120, 100)]
        public void Percentile_Successful(
            int[] data,
            int value,
            int expectation)
        {
            // Setup
            Array.Sort(data);

            // Act, Assert
            Assert.Equal(expectation, Math.Round(ComputePercentile.Percentile(data, value), 0));
        }
    }
}
