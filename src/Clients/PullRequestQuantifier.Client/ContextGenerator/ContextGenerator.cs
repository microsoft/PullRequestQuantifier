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
            var context = DefaultContext.Value.ShallowCopy();

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

            context.AdditionPercentile = AdditionDeletionPercentile(historicalChanges, true);
            context.DeletionPercentile = AdditionDeletionPercentile(historicalChanges, false);
            context.FormulaPercentile = FormulaPercentile(historicalChanges);

            return context;
        }

        private SortedDictionary<int, float> AdditionDeletionPercentile(
            IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> historicalChanges,
            bool addition)
        {
            var data = new int[historicalChanges.Count];
            int idx = 0;
            foreach (var historicalChange in historicalChanges)
            {
                data[idx++] = historicalChange.Value.Sum(v => addition ? v.QuantifiedLinesAdded : v.QuantifiedLinesDeleted);
            }

            return Percentile(data);
        }

        private SortedDictionary<int, float> Percentile(int[] data)
        {
            var ret = new SortedDictionary<int, float>();
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

        private IEnumerable<(ThresholdFormula, SortedDictionary<int, float>)> FormulaPercentile(
            IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> historicalChanges)
        {
            var ret = new List<(ThresholdFormula, SortedDictionary<int, float>)>();
            foreach (ThresholdFormula thresholdFormula in (ThresholdFormula[])Enum.GetValues(typeof(ThresholdFormula)))
            {
                ret.Add((thresholdFormula, Percentile(GroupCommits(historicalChanges, thresholdFormula))));
            }

            return ret;
        }

        private int[] GroupCommits(
            IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> historicalChanges,
            ThresholdFormula thresholdFormula)
        {
            var data = new int[historicalChanges.Count];
            int idx = 0;
            foreach (var historicalChange in historicalChanges)
            {
                var value = thresholdFormula switch
                {
                    ThresholdFormula.Sum => historicalChange.Value.Sum(v =>
                        v.QuantifiedLinesAdded + v.QuantifiedLinesDeleted),
                    ThresholdFormula.Avg => historicalChange.Value.Sum(v =>
                        (v.QuantifiedLinesAdded + v.QuantifiedLinesDeleted) / 2),
                    ThresholdFormula.Min => historicalChange.Value.Sum(v =>
                        Math.Min(v.QuantifiedLinesAdded, v.QuantifiedLinesDeleted)),
                    ThresholdFormula.Max => historicalChange.Value.Sum(v =>
                        Math.Max(v.QuantifiedLinesAdded, v.QuantifiedLinesDeleted)),
                    _ => throw new ArgumentOutOfRangeException(nameof(thresholdFormula), thresholdFormula, null)
                };

                data[idx++] = value;
            }

            return data;
        }
    }
}