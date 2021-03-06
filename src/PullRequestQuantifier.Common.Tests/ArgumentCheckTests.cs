namespace PullRequestQuantifier.Common.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class ArgumentCheckTests
    {
        [Fact]
        public void ParameterIsNotNullPositive()
        {
            // act
            ArgumentCheck.ParameterIsNotNull("value", nameof(ArgumentCheck));

            // assert that we don't throw exception
            Assert.True(true);
        }

        [Fact]
        public void ParameterIsNotNullNegative()
        {
            // act, assert
            Assert.Throws<ArgumentNullException>(() => ArgumentCheck.ParameterIsNotNull(null, nameof(ArgumentCheck)));
        }

        [Fact]
        public void StringIsNotNullOrWhiteSpacePositive()
        {
            // act
            ArgumentCheck.StringIsNotNullOrWhiteSpace("value", nameof(ArgumentCheck));

            // assert that we don't throw exception
            Assert.True(true);
        }

        [Fact]
        public void StringIsNotNullOrWhiteSpaceNegative()
        {
            // act, assert
            Assert.Throws<ArgumentException>(() => ArgumentCheck.StringIsNotNullOrWhiteSpace(null, nameof(ArgumentCheck)));
        }

        [Fact]
        public void IntegerIsGreaterThenThresholdPositive()
        {
            // act
            ArgumentCheck.IntegerIsGreaterThenThreshold(3, nameof(ArgumentCheck), 0);

            // assert that we don't throw exception
            Assert.True(true);
        }

        [Fact]
        public void IntegerIsGreaterThenThresholdNegative()
        {
            // act, assert
            Assert.Throws<ArgumentException>(() => ArgumentCheck.IntegerIsGreaterThenThreshold(0, nameof(ArgumentCheck), 3));
        }

        [Fact]
        public void StringMatchesCaseInsensitivePositive()
        {
            // act
            ArgumentCheck.StringMatchesCaseInsensitive("Value", "value", nameof(ArgumentCheck));

            // assert that we don't throw exception
            Assert.True(true);
        }

        [Fact]
        public void StringMatchesCaseInsensitiveNegative()
        {
            // act, assert
            Assert.Throws<ArgumentException>(() => ArgumentCheck.StringMatchesCaseInsensitive("Value1", "value", nameof(ArgumentCheck)));
        }

        [Fact]
        public void NonEmptyGuidPositive()
        {
            // act
            ArgumentCheck.NonEmptyGuid(Guid.NewGuid(), nameof(ArgumentCheck));

            // assert that we don't throw exception
            Assert.True(true);
        }

        [Fact]
        public void NonEmptyGuidNegative()
        {
            // act, assert
            Assert.Throws<ArgumentException>(() => ArgumentCheck.NonEmptyGuid("value", nameof(ArgumentCheck)));
        }
    }
}
