namespace PullRequestQuantifier.Client.Tests
{
    using System;
    using PullRequestQuantifier.Client.Helpers;
    using Xunit;

    public sealed class QuantifyClientHelperTests
    {
        [Fact]
        public void GetColorTest()
        {
            // act, assert
            Assert.True(QuantifyClientHelper.GetColor("Red").Equals(ConsoleColor.Red));
        }
    }
}
