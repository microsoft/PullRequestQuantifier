namespace PrQuantifier.Core
{
    using System;

    public static class ComputePercentile
    {
        /// <summary>
        /// Compute the percentile position of t he value within  the data array.
        /// In other words showing the percent of values form data array bellow our given value.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="value">The given value.</param>
        /// <returns>returns the percentile.</returns>
        public static float Percentile(int[] data, int value)
        {
            Array.Sort(data);
            var maxValue = data[^1];
            var minValue = data[0];

            if (value > maxValue)
            {
                return 100f;
            }

            if (value < minValue)
            {
                return 0f;
            }

            int idxBellowValues = Array.FindIndex(data, d => value <= d);

            return idxBellowValues / (data.Length - 1f) * 100f;
        }
    }
}
