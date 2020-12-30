namespace PullRequestQuantifier
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;
    using global::PullRequestQuantifier.Abstractions.Git;

    public sealed class PullRequestQuantifier : IPullRequestQuantifier
    {
        public PullRequestQuantifier(Context context)
        {
            Context = context;
        }

        public Context Context { get; }

        /// <inheritdoc />
        public async Task<QuantifierResult> Quantify(QuantifierInput quantifierInput)
        {
            if (quantifierInput == null)
            {
                throw new ArgumentNullException(nameof(quantifierInput));
            }

            // execute quantifier for this context and this particular input
            return await Compute(quantifierInput);
        }

        private async Task<QuantifierResult> Compute(QuantifierInput quantifierInput)
        {
            QuantifierResult quantifierResult = null;

            await Task.Factory.StartNew(() =>
            {
                quantifierResult = new QuantifierResult
                {
                    QuantifierInput = quantifierInput
                };

                // involve context and compute
                foreach (GitFilePatch quantifierInputChange in quantifierInput.Changes)
                {
                    ApplyContext(quantifierInputChange);
                }

                CountTotalChanges(quantifierInput, quantifierResult);

                // compute the label using the context percentile information and the thresholds
                SetLabel(quantifierResult);
            });

            return quantifierResult;
        }

        private void ApplyContext(GitFilePatch quantifierInputChange)
        {
            if (quantifierInputChange.RemoveNotIncludedOrExcluded(Context.Included, Context.Excluded))
            {
                // no need to evaluate remaining context if file is already excluded
                return;
            }

            // no language context found continue without
            if (Context.LanguageOptions != null)
            {
                if (Context.LanguageOptions.IgnoreSpaces)
                {
                    quantifierInputChange.RemoveWhiteSpacesChanges();
                }

                if (Context.LanguageOptions.IgnoreComments)
                {
                    quantifierInputChange.RemoveCommentsChanges();
                }

                if (Context.LanguageOptions.IgnoreCodeBlockSeparator)
                {
                    quantifierInputChange.RemoveCodeBlockSeparatorChanges();
                }
            }

            quantifierInputChange.RemoveRenamedChanges();
            quantifierInputChange.RemoveCopiedChanges();

            // do the final count of changes
            quantifierInputChange.ComputeChanges();
        }

        private void CountTotalChanges(
            QuantifierInput quantifierInput,
            QuantifierResult quantifierResult)
        {
            quantifierResult.QuantifiedLinesAdded = quantifierInput.Changes.Sum(c => c.QuantifiedLinesAdded);
            quantifierResult.QuantifiedLinesDeleted = quantifierInput.Changes.Sum(c => c.QuantifiedLinesDeleted);
        }

        private void SetLabel(QuantifierResult quantifierResult)
        {
            // in case no addition/deletion found then we won't be able to set the label.
            if (Context.AdditionPercentile == null
                || Context.DeletionPercentile == null
                || Context.AdditionPercentile.Count == 0
                || Context.DeletionPercentile.Count == 0)
            {
                return;
            }

            quantifierResult.PercentileAddition = MathF.Round(GetPercentile(quantifierResult, true), 2);
            quantifierResult.PercentileDeletion = MathF.Round(GetPercentile(quantifierResult, false), 2);

            if (quantifierResult.QuantifiedLinesDeleted == 0 && quantifierResult.QuantifiedLinesAdded == 0)
            {
                quantifierResult.Label = "No Changes";
                quantifierResult.Color = nameof(Color.Green);
                return;
            }

            // for now to set the label use the absolute values addition/deletion and compare them with the thresholds
            // the percentile will be displayed saying that if your change has this number
            // of lines additions then you are at this percentile within this context
            foreach (var contextThreshold in Context.Thresholds.OrderBy(t => t.Value))
            {
                // we set the label from the thresholds and exit when we have first value threshold grater then percentile
                quantifierResult.Label = contextThreshold.Label;
                quantifierResult.Color = contextThreshold.Color;
                quantifierResult.Formula = contextThreshold.Formula;

                if (GetChangeNumber(quantifierResult, contextThreshold.Formula) <= contextThreshold.Value)
                {
                    return;
                }
            }
        }

        private float GetPercentile(
            QuantifierResult quantifierResult,
            bool addition)
        {
            var operationValues = addition
                ? Context.AdditionPercentile.Keys.ToArray()
                : Context.DeletionPercentile.Keys.ToArray();

            var idxUpperBound = Array.FindIndex(
                operationValues,
                arrayElement =>
                    (addition ? quantifierResult.QuantifiedLinesAdded : quantifierResult.QuantifiedLinesDeleted) <=
                    arrayElement);

            var idxLowerBound = Array.FindLastIndex(
                operationValues,
                arrayElement =>
                    arrayElement <= (addition
                        ? quantifierResult.QuantifiedLinesAdded
                        : quantifierResult.QuantifiedLinesDeleted));

            var lowerBoundPercentile =
                (addition ? quantifierResult.QuantifiedLinesAdded : quantifierResult.QuantifiedLinesDeleted) <
                operationValues[0]
                    ? 0
                    : (addition
                        ? Context.AdditionPercentile[operationValues[idxLowerBound]]
                        : Context.DeletionPercentile[operationValues[idxLowerBound]]);

            var upperBoundPercentile =
                (addition ? quantifierResult.QuantifiedLinesAdded : quantifierResult.QuantifiedLinesDeleted) >
                operationValues[^1]
                    ? 100
                    : (addition
                        ? Context.AdditionPercentile[operationValues[idxUpperBound]]
                        : Context.DeletionPercentile[operationValues[idxUpperBound]]);

            // todo here change this and compute accurately
            return Math.Min(lowerBoundPercentile, upperBoundPercentile);
        }

        private int GetChangeNumber(
            QuantifierResult quantifierResult,
            ThresholdFormula formula)
        {
            return formula switch
            {
                ThresholdFormula.Sum => quantifierResult.QuantifiedLinesAdded + quantifierResult.QuantifiedLinesDeleted,
                ThresholdFormula.Avg => (quantifierResult.QuantifiedLinesAdded + quantifierResult.QuantifiedLinesDeleted) / 2,
                ThresholdFormula.Min => Math.Min(quantifierResult.QuantifiedLinesAdded, quantifierResult.QuantifiedLinesDeleted),
                ThresholdFormula.Max => Math.Max(quantifierResult.QuantifiedLinesAdded, quantifierResult.QuantifiedLinesDeleted),
                _ => 0
            };
        }
    }
}