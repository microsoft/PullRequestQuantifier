namespace PullRequestQuantifier.Abstractions.Tests.Context
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using PullRequestQuantifier.Abstractions.Context;
    using PullRequestQuantifier.Abstractions.Exceptions;
    using PullRequestQuantifier.Abstractions.Git;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class ContextTests
    {
        [Fact]
        public void Validate_NothingSpecified_ReturnsInvalidContext()
        {
            // Setup
            var context = new Context();

            // Act, Assert
            Assert.Throws<ThresholdException>(() => context.Validate());
        }

        [Fact]
        public void Validate_ThresholdsOutOfBound_ReturnsInvalidContext()
        {
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            {
                new Threshold { Value = 2000, Label = "1" },
                new Threshold { Value = 60, Label = "2" },
                new Threshold { Value = 50, Label = "3" }
            };
            context.Thresholds = thresholds;

            // Act, Assert
            Assert.Throws<ThresholdException>(() => context.Validate());
        }

        [Fact]
        public void Validate_ThresholdsCountLessThanThree_ReturnsInvalidContext()
        {
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            {
                new Threshold { Value = 200, Label = "1" },
                new Threshold { Value = 50, Label = "3" }
            };
            context.Thresholds = thresholds;

            // Act, Assert
            Assert.Throws<ThresholdException>(() => context.Validate());
        }

        [Fact]
        public void Validate_TwoThresholdsAreEqual_ReturnsInvalidContext()
        {
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            {
                new Threshold { Value = 100, Label = "1" },
                new Threshold { Value = 100, Label = "2" },
                new Threshold { Value = 50, Label = "3" }
            };
            context.Thresholds = thresholds;

            // Act, Assert
            Assert.Throws<ThresholdException>(() => context.Validate());
        }

        [Fact]
        public void Validate_EmptyLabel_ReturnsInvalidContext()
        {
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            {
                new Threshold { Value = 100, Label = "1" },
                new Threshold { Value = 60 },
                new Threshold { Value = 50, Label = "3" }
            };
            context.Thresholds = thresholds;

            // Act, Assert
            Assert.Throws<ThresholdException>(() => context.Validate());
        }

        [Fact]
        public void Validate_ReturnsValidContext()
        {
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            {
                new Threshold { Value = 100, Label = "1", GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete } },
                new Threshold { Value = 60, Label = "2" },
                new Threshold { Value = 50, Label = "3" }
            };
            context.Thresholds = thresholds;
            context.GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete };
            context.LanguageOptions = new LanguageOptions();

            // Act, Assert
            Assert.True(context.Validate() != null);
        }

        [Theory]
        [InlineData("ContextModel1.yaml")]
        [InlineData("ContextModel2.yaml")]
        [InlineData("ContextModel3.yaml")]
        [InlineData("ContextModel4.yaml")]
        [InlineData("ContextModel5.yaml")]
        [InlineData("ContextModel6.yaml")]
        public void Load_ReturnsValidContext(string file)
        {
            // Act, Assert
            Assert.True(ContextFactory.Load(@$"Data\{file}") != null);
        }
    }
}
