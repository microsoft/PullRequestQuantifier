namespace PullRequestQuantifier.Client.ContextGenerator
***REMOVED***
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;
    using global::PullRequestQuantifier.Abstractions.Git;
    using global::PullRequestQuantifier.GitEngine;

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
            var context = DefaultContext.Value;

            // do the quantification based on th default context to get accurate numbers
            IPullRequestQuantifier prQuantifier = new PullRequestQuantifier(context);
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
***REMOVED***
***REMOVED***