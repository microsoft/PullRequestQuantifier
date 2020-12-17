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
            var maxValue = data[^1];
            var minValue = data[0];

            // if our value is greater then the  max value then we are at 100 percentile
            if (value > maxValue)
            ***REMOVED***
                return 100f;
    ***REMOVED***

            // if it's less then we are at zero
            if (value < minValue)
            ***REMOVED***
                return 0f;
    ***REMOVED***

            // get the number of values less or equal then our value for wich we calculate the percentile
            int idxBellowValues = Array.FindLastIndex(data, d => d <= value) + 1;

            return idxBellowValues / (data.Length + 0f) * 100f;
***REMOVED***
***REMOVED***
***REMOVED***
