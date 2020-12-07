namespace PrQuantifier.Tests
***REMOVED***
    using System.Collections.Generic;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Exceptions;
    using global::PrQuantifier.Core.Git;
    using Xunit;

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
                new Threshold ***REMOVED*** Value = 200, Label = "1" ***REMOVED***,
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
        public void Validate_Value100Missing_ReturnsInvalidContext()
        ***REMOVED***
            // Setup
            var context = new Context();
            var thresholds = new List<Threshold>
            ***REMOVED***
                new Threshold ***REMOVED*** Value = 80, Label = "1" ***REMOVED***,
                new Threshold ***REMOVED*** Value = 60, Label = "2" ***REMOVED***,
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

        [Fact]
        public void Load_ReturnsValidContext()
        ***REMOVED***
            // Act, Assert
            Assert.True(ContextFactory.Load(@"Data\ContextModel1.txt") != null);
            Assert.True(ContextFactory.Load(@"Data\ContextModel2.txt") != null);
            Assert.True(ContextFactory.Load(@"Data\ContextModel3.txt") != null);
            Assert.True(ContextFactory.Load(@"Data\ContextModel4.txt") != null);
***REMOVED***
***REMOVED***
***REMOVED***
