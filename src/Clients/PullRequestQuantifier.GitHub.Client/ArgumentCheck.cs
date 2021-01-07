namespace PullRequestQuantifier.GitHub.Client
***REMOVED***
    using System;

    /// <summary>
    /// Helper class to put argument checks into a single location.
    /// </summary>
    public static class ArgumentCheck
    ***REMOVED***
        /// <summary>
        /// Throw an ArgumentNullException if the parameter is null.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <param name="parameterName">The parameter name (use the nameof operator).</param>
        public static void ParameterIsNotNull(object parameter, string parameterName)
        ***REMOVED***
            if (parameter == null)
            ***REMOVED***
                throw new ArgumentNullException(parameterName, "Parameter must not be null");
    ***REMOVED***
***REMOVED***

        /// <summary>
        /// Throw an ArgumentException if the parameter is null or spaces only string.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <param name="parameterName">The parameter name (use the nameof operator).</param>
        public static void StringIsNotNullOrWhiteSpace(string parameter, string parameterName)
        ***REMOVED***
            if (string.IsNullOrWhiteSpace(parameter))
            ***REMOVED***
                throw new ArgumentException("Parameter must not be trivial", parameterName);
    ***REMOVED***
***REMOVED***

        /// <summary>
        /// Throw an ArgumentException if the parameter is less or equal than the threshold.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <param name="parameterName">The parameter name (use the nameof operator).</param>
        /// <param name="threshold">Threshold to compare with.</param>
        public static void IntegerIsGreaterThenThreshold(long parameter, string parameterName, long threshold)
        ***REMOVED***
            if (parameter <= threshold)
            ***REMOVED***
                throw new ArgumentException($"Parameter must be strictly greater than ***REMOVED***threshold***REMOVED***", parameterName);
    ***REMOVED***
***REMOVED***

        /// <summary>
        /// Throw an ArgumentException if the string does not match the expected value (case-insensitive check).
        /// </summary>
        /// <param name="expectedValue">The expected value of the parameter.</param>
        /// <param name="parameter">The parameter to check.</param>
        /// <param name="parameterName">The parameter name (use the nameof operator).</param>
        public static void StringMatchesCaseInsensitive(string expectedValue, string parameter, string parameterName)
        ***REMOVED***
            ParameterIsNotNull(expectedValue, nameof(expectedValue));
            if (!expectedValue.Equals(parameter, StringComparison.OrdinalIgnoreCase))
            ***REMOVED***
                throw new ArgumentException("Parameter does not match expected value", parameterName);
    ***REMOVED***
***REMOVED***

        public static void NonEmptyGuid(Guid parameter, string parameterName)
        ***REMOVED***
            if (parameter == Guid.Empty)
            ***REMOVED***
                throw new ArgumentException("Paramater must be non-empty Guid", parameterName);
    ***REMOVED***
***REMOVED***

        public static void NonEmptyGuid(
            string parameter,
            string parameterName,
            string format = "D")
        ***REMOVED***
            NonEmptyGuid(
                parameter,
                () => throw new ArgumentException("Parameter is not a valid non-empty Guid", parameterName),
                format);
***REMOVED***

        public static void NonEmptyGuid(
            string parameter,
            Action throwEx,
            string format = "D")
        ***REMOVED***
            ParameterIsNotNull(throwEx, nameof(throwEx));

            if (IsEmptyGuid(parameter, format))
            ***REMOVED***
                throwEx();
    ***REMOVED***
***REMOVED***

        public static bool IsEmptyGuid(string parameter, string format = "D")
        ***REMOVED***
            return !Guid.TryParseExact(parameter, format, out Guid result) || (result == Guid.Empty);
***REMOVED***

        public static void NonEmptyArray<T>(T[] parameter, string parameterName)
        ***REMOVED***
            ArgumentCheck.ParameterIsNotNull(parameter, parameterName);

            if (parameter.Length == 0)
            ***REMOVED***
                throw new ArgumentException("Non-empty array expected!", parameterName);
    ***REMOVED***
***REMOVED***
***REMOVED***
***REMOVED***
