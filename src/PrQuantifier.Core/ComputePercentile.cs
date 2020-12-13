namespace PrQuantifier.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MathNet.Numerics.Statistics;

    public sealed class ComputePercentile
    {
        public float Percentile(int[] data, int value)
        {
            Array.Sort(data);
            var maxValue = data[^1];
            var minValue = data[0];

            if (value > maxValue)
            {
                Array.Resize(ref data, data.Length + 1);
                data[^1] = value;
            }

            if (value < minValue)
            {
                Array.Resize(ref data, data.Length + 1);
                data[^1] = value;
                Array.Sort(data);
            }

            var percentData = MapData(data);

            return percentData.Percentile((int)MapValue(data, value));
        }

        private IEnumerable<float> MapData(int[] data)
        {
            return data.Select(d => MapValue(data, d)).ToList();
        }

        private float MapValue(int[] data, int value)
        {
            var maxValue = data[^1];
            var minValue = data[0];

            return (value - minValue) * 100f / (maxValue - minValue);
        }
    }
}
