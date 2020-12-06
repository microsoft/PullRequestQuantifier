namespace PrQuantifier.Tests
***REMOVED***
    using global::PrQuantifier.Core.Exceptions;
    using global::PrQuantifier.Core.Model;
    using Xunit;
    using YamlDotNet.Serialization;

    public sealed class ContextTests
    ***REMOVED***
        [Fact]
        public void Validate_NothingSpecified_ReturnsInvalidContext()
        ***REMOVED***
            // Setup
            var context = new Context();

            //Act, Assert
            Assert.Throws<ThresholdException>(()=>context.Validate());
***REMOVED***

        [Fact]
        public void Validate_ThresholdsOutOfBound_ReturnsInvalidContext()
        ***REMOVED***
            // Setup
            var context = new Context();
            context.Thresholds.Add(new Threshold***REMOVED***Value = 200,Label = "1"***REMOVED***);
            context.Thresholds.Add(new Threshold***REMOVED***Value = 60,Label = "2"***REMOVED***);
            context.Thresholds.Add(new Threshold***REMOVED***Value = 50,Label = "3"***REMOVED***);

            //Act, Assert
            Assert.Throws<ThresholdException>(()=>context.Validate());
***REMOVED***

        [Fact]
        public void Validate_ThresholdsCountLessThanThree_ReturnsInvalidContext()
        ***REMOVED***
            // Setup
            var context = new Context();
            context.Thresholds.Add(new Threshold***REMOVED***Value = 200,Label = "1"***REMOVED***);
            context.Thresholds.Add(new Threshold***REMOVED***Value = 50,Label = "3"***REMOVED***);

            //Act, Assert
            Assert.Throws<ThresholdException>(()=>context.Validate());
***REMOVED***

        [Fact]
        public void Validate_TwoThresholdsAreEqual_ReturnsInvalidContext()
        ***REMOVED***
            // Setup
            var context = new Context();
            context.Thresholds.Add(new Threshold***REMOVED***Value = 60,Label = "1"***REMOVED***);
            context.Thresholds.Add(new Threshold***REMOVED***Value = 60,Label = "2"***REMOVED***);
            context.Thresholds.Add(new Threshold***REMOVED***Value = 50,Label = "3"***REMOVED***);

            //Act, Assert
            Assert.Throws<ThresholdException>(()=>context.Validate());
***REMOVED***

        [Fact]
        public void Validate_EmptyLabel_ReturnsInvalidContext()
        ***REMOVED***
            // Setup
            var context = new Context();
            context.Thresholds.Add(new Threshold***REMOVED***Value = 60,Label = "1"***REMOVED***);
            context.Thresholds.Add(new Threshold***REMOVED***Value = 60***REMOVED***);
            context.Thresholds.Add(new Threshold***REMOVED***Value = 50,Label = "3"***REMOVED***);

            //Act, Assert
            Assert.Throws<ThresholdException>(()=>context.Validate());
***REMOVED***

        [Fact]
        public void Validate_ReturnsValidContext()
        ***REMOVED***
            // Setup
            var context = new Context();
            context.Thresholds.Add(new Threshold***REMOVED***Value = 80,Label = "1"***REMOVED***);
            context.Thresholds.Add(new Threshold***REMOVED***Value = 60,Label = "2"***REMOVED***);
            context.Thresholds.Add(new Threshold***REMOVED***Value = 50,Label = "3"***REMOVED***);

            //Act, Assert
            Assert.True(context.Validate()!=null);

            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(context);
***REMOVED***
***REMOVED***
***REMOVED***
