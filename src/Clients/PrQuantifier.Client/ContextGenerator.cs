namespace PrQuantifier.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core;
    using global::PrQuantifier.Core.Abstractions;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Git;

    public sealed class ContextGenerator : IContextGenerator
    {
        private readonly IGitEngine gitEngine;

        public ContextGenerator()
        {
            gitEngine = new GitEngine();
        }

        /// <inheritdoc />
        public async Task<Context> Create(string repoPath)
        {
            // get historical changes
            var historicalChanges = gitEngine.GetGitHistoricalChangesToParent(repoPath);
            var context = InitializeDefaultContext();

            // do the quantification based on th default context to get accurate numbers
            IPrQuantifier prQuantifier = new PrQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(historicalChanges.Values.SelectMany(v => v));
            await prQuantifier.Quantify(quantifierInput);

            context.AdditionPercentile = new SortedDictionary<int, float>(Percentile(historicalChanges, true));
            context.DeletionPercentile = new SortedDictionary<int, float>(Percentile(historicalChanges, false));

            return context;
        }

        private static IDictionary<int, float> Percentile(
            IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> historicalChanges,
            bool addition)
        {
            var ret = new Dictionary<int, float>();

            var data = historicalChanges
                .SelectMany(h => h.Value)
                .Where(v => (addition ? v.QuantifiedLinesAdded : v.QuantifiedLinesDeleted) > 0)
                .Select(v => addition ? v.QuantifiedLinesAdded : v.QuantifiedLinesDeleted).ToArray();
            Array.Sort(data);

            foreach (var value in data)
            {
                var percentile = ComputePercentile.Percentile(
                    data,
                    value);
                ret[value] = percentile;
            }

            return ret;
        }

        private static Context InitializeDefaultContext()
        {
            // todo use exiting contexts to augment with the new data.
            var context = new Context
            {
                LanguageOptions = new LanguageOptions
                {
                    IgnoreCodeBlockSeparator = true,
                    IgnoreComments = true,
                    IgnoreSpaces = true
                },
                DynamicBehaviour = false,
                Thresholds = new List<Threshold>
                {
                    new Threshold
                    {
                        Label = "Extra Small",
                        Value = 9,
                        Color = "Green",
                        GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                    },
                    new Threshold
                    {
                        Label = "Small",
                        Value = 29,
                        Color = "Green",
                        GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                    },
                    new Threshold
                    {
                        Label = "Medium",
                        Value = 99,
                        Color = "Yellow",
                        GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                    },
                    new Threshold
                    {
                        Label = "Large",
                        Value = 499,
                        Color = "Red",
                        GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                    },
                    new Threshold
                    {
                        Label = "Extra Large",
                        Value = 999,
                        Color = "Red",
                        GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                    }
                },
                Excluded = new List<string> { "*.csproj" },
                GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
            };

            return context;
        }
    }
}
