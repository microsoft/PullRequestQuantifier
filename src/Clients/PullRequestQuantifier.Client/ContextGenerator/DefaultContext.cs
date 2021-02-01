using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PullRequestQuantifier.Client.Tests")]

namespace PullRequestQuantifier.Client.ContextGenerator
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
                    Value = 30,
                    Color = "Green",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                },
                new Threshold
                {
                    Label = "Small",
                    Value = 70,
                    Color = "Green",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                },
                new Threshold
                {
                    Label = "Medium",
                    Value = 150,
                    Color = "DarkYellow",
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

            Value = new Context
            {
                LanguageOptions = new LanguageOptions
                {
                    IgnoreCodeBlockSeparator = true,
                    IgnoreComments = true,
                    IgnoreSpaces = true
                },
                DynamicBehaviour = false,
                IgnoreRenamed = true,
                IgnoreCopied = true,
                Thresholds = defaultThresholds,
                Excluded = new List<string>
                {
                    "*.csproj",
                    "prquantifier.yaml",
                    "package-lock.json",
                    "packages.lock.json",
                    "*.md",
                    "*.sln",
                    "*.snap"
                },
                GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete },
                AdditionPercentile = DefaultPercentile(defaultThresholds.OrderBy(t => t.Value)),
                DeletionPercentile = DefaultPercentile(defaultThresholds.OrderBy(t => t.Value)),
                FormulaPercentile = FormulaPercentile(defaultThresholds.OrderBy(t => t.Value))
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

        private static IEnumerable<(ThresholdFormula, SortedDictionary<int, float>)> FormulaPercentile(IEnumerable<Threshold> thresholds)
        {
            var ret = new List<(ThresholdFormula, SortedDictionary<int, float>)>();
            var enumerable = thresholds as Threshold[] ?? thresholds.ToArray();
            var formulas = enumerable.Select(t => t.Formula).Distinct();

            foreach (var thresholdFormula in formulas)
            {
                ret.Add((thresholdFormula, DefaultPercentile(enumerable)));
            }

            return ret;
        }
    }
}