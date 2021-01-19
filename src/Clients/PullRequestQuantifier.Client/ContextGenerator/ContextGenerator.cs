namespace PullRequestQuantifier.Client.ContextGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;
    using global::PullRequestQuantifier.Abstractions.Git;
    using global::PullRequestQuantifier.GitEngine;

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
            var context = DefaultContext.Value;

            // if there are no history changes into the repo then return the default context.
            if (historicalChanges.Count == 0)
            {
                return context;
            }

            // do the quantification based on th default context to get accurate numbers
            IPullRequestQuantifier prQuantifier = new PullRequestQuantifier(context);
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

            var data = new int[historicalChanges.Count];
            int idx = 0;
            foreach (var historicalChange in historicalChanges)
            {
                data[idx++] = historicalChange.Value.Sum(v => addition ? v.QuantifiedLinesAdded : v.QuantifiedLinesDeleted);
            }

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
    }
}