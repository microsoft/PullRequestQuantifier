namespace PrQuantifier.Tests
{
    using global::PrQuantifier.Core.Exceptions;
    using global::PrQuantifier.Core.Model;
    using Xunit;
    using YamlDotNet.Serialization;

    public sealed class ContextTests
    {
        [Fact]
        public void Validate_NothingSpecified_ReturnsInvalidContext()
        {
            // Setup
            var context = new Context();

            //Act, Assert
            Assert.Throws<ThresholdException>(()=>context.Validate());
        }

        [Fact]
        public void Validate_ThresholdsOutOfBound_ReturnsInvalidContext()
        {
            // Setup
            var context = new Context();
            context.Thresholds.Add(new Threshold{Value = 200,Label = "1"});
            context.Thresholds.Add(new Threshold{Value = 60,Label = "2"});
            context.Thresholds.Add(new Threshold{Value = 50,Label = "3"});

            //Act, Assert
            Assert.Throws<ThresholdException>(()=>context.Validate());
        }

        [Fact]
        public void Validate_ThresholdsCountLessThanThree_ReturnsInvalidContext()
        {
            // Setup
            var context = new Context();
            context.Thresholds.Add(new Threshold{Value = 200,Label = "1"});
            context.Thresholds.Add(new Threshold{Value = 50,Label = "3"});

            //Act, Assert
            Assert.Throws<ThresholdException>(()=>context.Validate());
        }

        [Fact]
        public void Validate_TwoThresholdsAreEqual_ReturnsInvalidContext()
        {
            // Setup
            var context = new Context();
            context.Thresholds.Add(new Threshold{Value = 60,Label = "1"});
            context.Thresholds.Add(new Threshold{Value = 60,Label = "2"});
            context.Thresholds.Add(new Threshold{Value = 50,Label = "3"});

            //Act, Assert
            Assert.Throws<ThresholdException>(()=>context.Validate());
        }

        [Fact]
        public void Validate_EmptyLabel_ReturnsInvalidContext()
        {
            // Setup
            var context = new Context();
            context.Thresholds.Add(new Threshold{Value = 60,Label = "1"});
            context.Thresholds.Add(new Threshold{Value = 60});
            context.Thresholds.Add(new Threshold{Value = 50,Label = "3"});

            //Act, Assert
            Assert.Throws<ThresholdException>(()=>context.Validate());
        }

        [Fact]
        public void Validate_ReturnsValidContext()
        {
            // Setup
            var context = new Context();
            context.Thresholds.Add(new Threshold{Value = 80,Label = "1"});
            context.Thresholds.Add(new Threshold{Value = 60,Label = "2"});
            context.Thresholds.Add(new Threshold{Value = 50,Label = "3"});

            //Act, Assert
            Assert.True(context.Validate()!=null);

            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(context);
        }
    }
}
