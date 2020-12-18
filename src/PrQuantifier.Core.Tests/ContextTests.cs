namespace PrQuantifier.Core.Tests
***REMOVED***
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using PrQuantifier.Core.Context;
    using PrQuantifier.Core.Exceptions;
    using PrQuantifier.Core.Git;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class ContextTests
    ***REMOVED***
        [Fact]
        public void Validate_NothingSpecified_ReturnsInvalidContext()
        ***REMOVED***
            // Setup
            var context = new Context();

            // Act, Assert
            Assert.Throws<ThresholdException>(() => context.Validate());
***REMOVED***

        [Fact]
        public void Validate_ThresholdsOutOfBound_ReturnsInvalidContext()
        ***REMOVED***
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            ***REMOVED***
                new Threshold ***REMOVED*** Value = 2000, Label = "1" ***REMOVED***,
                new Threshold ***REMOVED*** Value = 60, Label = "2" ***REMOVED***,
                new Threshold ***REMOVED*** Value = 50, Label = "3" ***REMOVED***
    ***REMOVED***;
            context.Thresholds = thresholds;

            // Act, Assert
            Assert.Throws<ThresholdException>(() => context.Validate());
***REMOVED***

        [Fact]
        public void Validate_ThresholdsCountLessThanThree_ReturnsInvalidContext()
        ***REMOVED***
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            ***REMOVED***
                new Threshold ***REMOVED*** Value = 200, Label = "1" ***REMOVED***,
                new Threshold ***REMOVED*** Value = 50, Label = "3" ***REMOVED***
    ***REMOVED***;
            context.Thresholds = thresholds;

            // Act, Assert
            Assert.Throws<ThresholdException>(() => context.Validate());
***REMOVED***

        [Fact]
        public void Validate_TwoThresholdsAreEqual_ReturnsInvalidContext()
        ***REMOVED***
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            ***REMOVED***
                new Threshold ***REMOVED*** Value = 100, Label = "1" ***REMOVED***,
                new Threshold ***REMOVED*** Value = 100, Label = "2" ***REMOVED***,
                new Threshold ***REMOVED*** Value = 50, Label = "3" ***REMOVED***
    ***REMOVED***;
            context.Thresholds = thresholds;

            // Act, Assert
            Assert.Throws<ThresholdException>(() => context.Validate());
***REMOVED***

        [Fact]
        public void Validate_EmptyLabel_ReturnsInvalidContext()
        ***REMOVED***
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            ***REMOVED***
                new Threshold ***REMOVED*** Value = 100, Label = "1" ***REMOVED***,
                new Threshold ***REMOVED*** Value = 60 ***REMOVED***,
                new Threshold ***REMOVED*** Value = 50, Label = "3" ***REMOVED***
    ***REMOVED***;
            context.Thresholds = thresholds;

            // Act, Assert
            Assert.Throws<ThresholdException>(() => context.Validate());
***REMOVED***

        [Fact]
        public void Validate_ReturnsValidContext()
        ***REMOVED***
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            ***REMOVED***
                new Threshold ***REMOVED*** Value = 100, Label = "1", GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED*** ***REMOVED***,
                new Threshold ***REMOVED*** Value = 60, Label = "2" ***REMOVED***,
                new Threshold ***REMOVED*** Value = 50, Label = "3" ***REMOVED***
    ***REMOVED***;
            context.Thresholds = thresholds;
            context.GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***;
            context.LanguageOptions = new LanguageOptions();

            // Act, Assert
            Assert.True(context.Validate() != null);
***REMOVED***

        [Theory]
        [InlineData("ContextModel1.yaml")]
        [InlineData("ContextModel2.yaml")]
        [InlineData("ContextModel3.yaml")]
        [InlineData("ContextModel4.yaml")]
        [InlineData("ContextModel5.yaml")]
        [InlineData("ContextModel6.yaml")]
        public void Load_ReturnsValidContext(string file)
        ***REMOVED***
            // Act, Assert
            Assert.True(ContextFactory.Load(@$"Data\***REMOVED***file***REMOVED***") != null);
***REMOVED***
***REMOVED***
***REMOVED***
