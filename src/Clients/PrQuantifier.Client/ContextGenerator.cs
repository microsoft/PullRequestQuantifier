namespace PrQuantifier.Client
***REMOVED***
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core;
    using global::PrQuantifier.Core.Abstractions;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Git;

    public sealed class ContextGenerator : IContextGenerator
    ***REMOVED***
        private readonly IGitEngine gitEngine;

        public ContextGenerator()
        ***REMOVED***
            gitEngine = new GitEngine();
***REMOVED***

        /// <inheritdoc />
        public async Task<Context> Create(string repoPath)
        ***REMOVED***
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
***REMOVED***

        private static IDictionary<int, float> Percentile(
            IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> historicalChanges,
            bool addition)
        ***REMOVED***
            var ret = new Dictionary<int, float>();

            var data = historicalChanges
                .SelectMany(h => h.Value)
                .Where(v => (addition ? v.QuantifiedLinesAdded : v.QuantifiedLinesDeleted) > 0)
                .Select(v => addition ? v.QuantifiedLinesAdded : v.QuantifiedLinesDeleted).ToArray();
            Array.Sort(data);

            foreach (var value in data)
            ***REMOVED***
                var percentile = ComputePercentile.Percentile(
                    data,
                    value);
                ret[value] = percentile;
    ***REMOVED***

            return ret;
***REMOVED***

        private static Context InitializeDefaultContext()
        ***REMOVED***
            // todo use exiting contexts to augment with the new data.
            var context = new Context
            ***REMOVED***
                LanguageOptions = new LanguageOptions
                ***REMOVED***
                    IgnoreCodeBlockSeparator = true,
                    IgnoreComments = true,
                    IgnoreSpaces = true
        ***REMOVED***,
                DynamicBehaviour = false,
                Thresholds = new List<Threshold>
                ***REMOVED***
                    new Threshold
                    ***REMOVED***
                        Label = "Extra Small",
                        Value = 9,
                        Color = "Green",
                        GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
            ***REMOVED***,
                    new Threshold
                    ***REMOVED***
                        Label = "Small",
                        Value = 29,
                        Color = "Green",
                        GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
            ***REMOVED***,
                    new Threshold
                    ***REMOVED***
                        Label = "Medium",
                        Value = 99,
                        Color = "Yellow",
                        GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
            ***REMOVED***,
                    new Threshold
                    ***REMOVED***
                        Label = "Large",
                        Value = 499,
                        Color = "Red",
                        GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
            ***REMOVED***,
                    new Threshold
                    ***REMOVED***
                        Label = "Extra Large",
                        Value = 999,
                        Color = "Red",
                        GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
            ***REMOVED***
        ***REMOVED***,
                Excluded = new List<string> ***REMOVED*** "*.csproj" ***REMOVED***,
                GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
    ***REMOVED***;

            return context;
***REMOVED***
***REMOVED***
***REMOVED***
