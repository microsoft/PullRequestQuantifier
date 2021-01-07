using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PullRequestQuantifier.Client.Tests")]

namespace PullRequestQuantifier.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Git;

    public static class DefaultContext
    {
        internal static readonly Context Value;

        static DefaultContext()
        {
            var defaultThresholds = new List<Threshold>
            {
                new Threshold
                {
                    Label = "Extra Small",
                    Value = 10,
                    Color = "Green",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                },
                new Threshold
                {
                    Label = "Small",
                    Value = 40,
                    Color = "Green",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                },
                new Threshold
                {
                    Label = "Medium",
                    Value = 100,
                    Color = "Yellow",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                },
                new Threshold
                {
                    Label = "Large",
                    Value = 400,
                    Color = "Red",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                },
                new Threshold
                {
                    Label = "Extra Large",
                    Value = 1000,
                    Color = "Red",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                }
            };

            var defaultPercentileValue = DefaultPercentile(defaultThresholds.OrderBy(t => t.Value));

            Value = new Context
            {
                LanguageOptions = new LanguageOptions
                {
                    IgnoreCodeBlockSeparator = true,
                    IgnoreComments = true,
                    IgnoreSpaces = true
                },
                DynamicBehaviour = false,
                Thresholds = defaultThresholds,
                Excluded = new List<string> { "*.csproj" },
                GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete },
                AdditionPercentile = defaultPercentileValue,
                DeletionPercentile = defaultPercentileValue
            };
        }

        private static SortedDictionary<int, float> DefaultPercentile(IEnumerable<Threshold> thresholds)
        {
            short lowerBound = 0;
            var ret = new SortedDictionary<int, float> { { lowerBound, 0 } };

            var thresholdsArray = thresholds as Threshold[] ?? thresholds.ToArray();
            float bucketRange = 100f / thresholdsArray.Length;

            foreach (var threshold in thresholdsArray)
            {
                float additionValue = bucketRange / (threshold.Value - lowerBound);

                while (++lowerBound <= threshold.Value)
                {
                    ret[lowerBound] = ret[lowerBound - 1] + additionValue;
                }

                // reset lower bound to the last threshold
                lowerBound = threshold.Value;
            }

            return ret;
        }
    }
}