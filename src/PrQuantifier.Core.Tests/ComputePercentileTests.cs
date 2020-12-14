namespace PrQuantifier.Core.Tests
{
    using System;
    using Xunit;

    public sealed class ComputePercentileTests
    {
        [Theory]
        [InlineData(new[] { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 92, 100)]
        [InlineData(new[] { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 5, 11)]
        [InlineData(new[] { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 50, 56)]
        [InlineData(new[] { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 0, 0)]
        [InlineData(new[] { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90 }, 120, 100)]
        public void Percentile_Successful(
            int[] data,
            int value,
            int expectation)
        {
            // Act, Assert
            Assert.Equal(expectation, Math.Round(ComputePercentile.Percentile(data, value), 0));
        }
    }
}
