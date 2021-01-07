using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PullRequestQuantifier.Client.Tests")]

namespace PullRequestQuantifier.Client
***REMOVED***
    using System.Collections.Generic;
    using System.Linq;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Git;

    public static class DefaultContext
    ***REMOVED***
        internal static readonly Context Value;

        static DefaultContext()
        ***REMOVED***
            var defaultThresholds = new List<Threshold>
            ***REMOVED***
                new Threshold
                ***REMOVED***
                    Label = "Extra Small",
                    Value = 10,
                    Color = "Green",
                    GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
        ***REMOVED***,
                new Threshold
                ***REMOVED***
                    Label = "Small",
                    Value = 40,
                    Color = "Green",
                    GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
        ***REMOVED***,
                new Threshold
                ***REMOVED***
                    Label = "Medium",
                    Value = 100,
                    Color = "Yellow",
                    GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
        ***REMOVED***,
                new Threshold
                ***REMOVED***
                    Label = "Large",
                    Value = 400,
                    Color = "Red",
                    GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
        ***REMOVED***,
                new Threshold
                ***REMOVED***
                    Label = "Extra Large",
                    Value = 1000,
                    Color = "Red",
                    GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
        ***REMOVED***
    ***REMOVED***;

            var defaultPercentileValue = DefaultPercentile(defaultThresholds.OrderBy(t => t.Value));

            Value = new Context
            ***REMOVED***
                LanguageOptions = new LanguageOptions
                ***REMOVED***
                    IgnoreCodeBlockSeparator = true,
                    IgnoreComments = true,
                    IgnoreSpaces = true
        ***REMOVED***,
                DynamicBehaviour = false,
                Thresholds = defaultThresholds,
                Excluded = new List<string> ***REMOVED*** "*.csproj" ***REMOVED***,
                GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***,
                AdditionPercentile = defaultPercentileValue,
                DeletionPercentile = defaultPercentileValue
    ***REMOVED***;
***REMOVED***

        private static SortedDictionary<int, float> DefaultPercentile(IEnumerable<Threshold> thresholds)
        ***REMOVED***
            short lowerBound = 0;
            var ret = new SortedDictionary<int, float> ***REMOVED*** ***REMOVED*** lowerBound, 0 ***REMOVED*** ***REMOVED***;

            var thresholdsArray = thresholds as Threshold[] ?? thresholds.ToArray();
            float bucketRange = 100f / thresholdsArray.Length;

            foreach (var threshold in thresholdsArray)
            ***REMOVED***
                float additionValue = bucketRange / (threshold.Value - lowerBound);

                while (++lowerBound <= threshold.Value)
                ***REMOVED***
                    ret[lowerBound] = ret[lowerBound - 1] + additionValue;
        ***REMOVED***

                // reset lower bound to the last threshold
                lowerBound = threshold.Value;
    ***REMOVED***

            return ret;
***REMOVED***
***REMOVED***
***REMOVED***