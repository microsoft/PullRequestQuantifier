namespace PrQuantifier.Core
***REMOVED***
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MathNet.Numerics.Statistics;

    public sealed class ComputePercentile
    ***REMOVED***
        public float Percentile(int[] data, int value)
        ***REMOVED***
            Array.Sort(data);
            var maxValue = data[^1];
            var minValue = data[0];

            if (value > maxValue)
            ***REMOVED***
                Array.Resize(ref data, data.Length + 1);
                data[^1] = value;
    ***REMOVED***

            if (value < minValue)
            ***REMOVED***
                Array.Resize(ref data, data.Length + 1);
                data[^1] = value;
                Array.Sort(data);
    ***REMOVED***

            var percentData = MapData(data);

            return percentData.Percentile((int)MapValue(data, value));
***REMOVED***

        private IEnumerable<float> MapData(int[] data)
        ***REMOVED***
            return data.Select(d => MapValue(data, d)).ToList();
***REMOVED***

        private float MapValue(int[] data, int value)
        ***REMOVED***
            var maxValue = data[^1];
            var minValue = data[0];

            return (value - minValue) * 100f / (maxValue - minValue);
***REMOVED***
***REMOVED***
***REMOVED***
