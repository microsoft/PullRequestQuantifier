namespace PullRequestQuantifier.Client.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using PullRequestQuantifier.Client.Helpers;
    using Xunit;

    [ExcludeFromCodeCoverage]
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
