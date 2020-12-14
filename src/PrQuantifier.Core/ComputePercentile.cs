namespace PrQuantifier.Core
***REMOVED***
    using System;

    public static class ComputePercentile
    ***REMOVED***
        /// <summary>
        /// Compute the percentile position of t he value within  the data array.
        /// In other words showing the percent of values form data array bellow our given value.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="value">The given value.</param>
        /// <returns>returns the percentile.</returns>
        public static float Percentile(int[] data, int value)
        ***REMOVED***
            Array.Sort(data);
            var maxValue = data[^1];
            var minValue = data[0];

            if (value > maxValue)
            ***REMOVED***
                return 100f;
    ***REMOVED***

            if (value < minValue)
            ***REMOVED***
                return 0f;
    ***REMOVED***

            int idxBellowValues = Array.FindIndex(data, d => value <= d);

            return idxBellowValues / (data.Length - 1f) * 100f;
***REMOVED***
***REMOVED***
***REMOVED***
