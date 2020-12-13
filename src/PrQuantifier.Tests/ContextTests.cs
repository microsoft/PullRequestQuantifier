namespace PrQuantifier.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Exceptions;
    using global::PrQuantifier.Core.Git;
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
                new Threshold { Value = 200, Label = "1" },
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
        public void Validate_Value100Missing_ReturnsInvalidContext()
        {
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            {
                new Threshold { Value = 80, Label = "1" },
                new Threshold { Value = 60, Label = "2" },
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

        [Fact]
        public void Load_ReturnsValidContext()
        {
            // Act, Assert
            Assert.True(ContextFactory.Load(@"Data\ContextModel1.txt") != null);
            Assert.True(ContextFactory.Load(@"Data\ContextModel2.txt") != null);
            Assert.True(ContextFactory.Load(@"Data\ContextModel3.txt") != null);
            Assert.True(ContextFactory.Load(@"Data\ContextModel4.txt") != null);
        }
    }
}
