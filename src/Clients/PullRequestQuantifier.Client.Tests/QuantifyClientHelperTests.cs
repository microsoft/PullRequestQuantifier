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
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.Red)).Equals(ConsoleColor.Red));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.Black)).Equals(ConsoleColor.Black));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.DarkBlue)).Equals(ConsoleColor.DarkBlue));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.DarkGreen)).Equals(ConsoleColor.DarkGreen));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.DarkCyan)).Equals(ConsoleColor.DarkCyan));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.DarkRed)).Equals(ConsoleColor.DarkRed));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.DarkMagenta)).Equals(ConsoleColor.DarkMagenta));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.DarkYellow)).Equals(ConsoleColor.DarkYellow));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.Gray)).Equals(ConsoleColor.Gray));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.DarkGray)).Equals(ConsoleColor.DarkGray));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.Blue)).Equals(ConsoleColor.Blue));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.Green)).Equals(ConsoleColor.Green));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.Cyan)).Equals(ConsoleColor.Cyan));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.Magenta)).Equals(ConsoleColor.Magenta));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.Yellow)).Equals(ConsoleColor.Yellow));
            Assert.True(QuantifyClientHelper.GetColor(nameof(ConsoleColor.White)).Equals(ConsoleColor.White));
        }
    }
}
