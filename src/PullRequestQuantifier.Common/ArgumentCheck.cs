namespace PullRequestQuantifier.Common
{
    using System;

    /// <summary>
    /// Helper class to put argument checks into a single location.
    /// </summary>
    public static class ArgumentCheck
    {
        /// <summary>
        /// Throw an ArgumentNullException if the parameter is null.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <param name="parameterName">The parameter name (use the nameof operator).</param>
        public static void ParameterIsNotNull(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName, "Parameter must not be null");
            }
        }

        /// <summary>
        /// Throw an ArgumentException if the parameter is null or spaces only string.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <param name="parameterName">The parameter name (use the nameof operator).</param>
        public static void StringIsNotNullOrWhiteSpace(string parameter, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentException("Parameter must not be trivial", parameterName);
            }
        }

        /// <summary>
        /// Throw an ArgumentException if the parameter is less or equal than the threshold.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <param name="parameterName">The parameter name (use the nameof operator).</param>
        /// <param name="threshold">Threshold to compare with.</param>
        public static void IntegerIsGreaterThenThreshold(long parameter, string parameterName, long threshold)
        {
            if (parameter <= threshold)
            {
                throw new ArgumentException($"Parameter must be strictly greater than {threshold}", parameterName);
            }
        }

        /// <summary>
        /// Throw an ArgumentException if the string does not match the expected value (case-insensitive check).
        /// </summary>
        /// <param name="expectedValue">The expected value of the parameter.</param>
        /// <param name="parameter">The parameter to check.</param>
        /// <param name="parameterName">The parameter name (use the nameof operator).</param>
        public static void StringMatchesCaseInsensitive(string expectedValue, string parameter, string parameterName)
        {
            ParameterIsNotNull(expectedValue, nameof(expectedValue));
            if (!expectedValue.Equals(parameter, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Parameter does not match expected value", parameterName);
            }
        }

        public static void NonEmptyGuid(Guid parameter, string parameterName)
        {
            if (parameter == Guid.Empty)
            {
                throw new ArgumentException("Paramater must be non-empty Guid", parameterName);
            }
        }

        public static void NonEmptyGuid(
            string parameter,
            string parameterName,
            string format = "D")
        {
            NonEmptyGuid(
                parameter,
                () => throw new ArgumentException("Parameter is not a valid non-empty Guid", parameterName),
                format);
        }

        public static void NonEmptyGuid(
            string parameter,
            Action throwEx,
            string format = "D")
        {
            ParameterIsNotNull(throwEx, nameof(throwEx));

            if (IsEmptyGuid(parameter, format))
            {
                throwEx();
            }
        }

        public static bool IsEmptyGuid(string parameter, string format = "D")
        {
            return !Guid.TryParseExact(parameter, format, out Guid result) || (result == Guid.Empty);
        }

        public static void NonEmptyArray<T>(T[] parameter, string parameterName)
        {
            ArgumentCheck.ParameterIsNotNull(parameter, parameterName);

            if (parameter.Length == 0)
            {
                throw new ArgumentException("Non-empty array expected!", parameterName);
            }
        }
    }
}
