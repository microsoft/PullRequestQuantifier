namespace PullRequestQuantifier
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;
    using global::PullRequestQuantifier.Abstractions.Git;
    using global::PullRequestQuantifier.Common;

    public sealed class PullRequestQuantifier : IPullRequestQuantifier
    {
        public PullRequestQuantifier(Context context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets the Context. We allow the context to be set for reloading anytime when there is a new context.
        /// </summary>
        public Context Context { get; private set; }

        /// <inheritdoc />
        public async Task<QuantifierResult> Quantify(QuantifierInput quantifierInput)
        {
            ArgumentCheck.ParameterIsNotNull(quantifierInput, nameof(quantifierInput));

            // execute quantifier for this context and this particular input
            return await Compute(quantifierInput);
        }

        private async Task<QuantifierResult> Compute(QuantifierInput quantifierInput)
        {
            QuantifierResult quantifierResult = null;

            await Task.Factory.StartNew(
                () =>
                {
                    quantifierResult = new QuantifierResult
                    {
                        QuantifierInput = quantifierInput,
                        Context = Context
                    };

                    // involve context and compute
                    Parallel.ForEach(quantifierInput.Changes, ApplyContext);

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

            if (Context.IgnoreRenamed)
            {
                quantifierInputChange.RemoveRenamedChanges();
            }

            if (Context.IgnoreCopied)
            {
                quantifierInputChange.RemoveCopiedChanges();
            }

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
                quantifierResult.FormulaLinesChanged = GetChangeNumber(quantifierResult, contextThreshold.Formula);

                if (quantifierResult.FormulaLinesChanged <= contextThreshold.Value)
                {
                    break;
                }
            }

            // in case no addition/deletion found then we won't be able to set the change percentile.
            if (Context.AdditionPercentile == null
                || Context.DeletionPercentile == null
                || Context.FormulaPercentile == null
                || Context.AdditionPercentile.Count == 0
                || Context.DeletionPercentile.Count == 0
                || !Context.FormulaPercentile.Any())
            {
                return;
            }

            quantifierResult.PercentileAddition = MathF.Round(GetAdditionDeletionPercentile(quantifierResult, true), 2);
            quantifierResult.PercentileDeletion = MathF.Round(
                GetAdditionDeletionPercentile(quantifierResult, false),
                2);
            quantifierResult.FormulaPercentile = MathF.Round(GetFormulaPercentile(quantifierResult), 2);
        }

        private float GetAdditionDeletionPercentile(
            QuantifierResult quantifierResult,
            bool addition)
        {
            return addition
                ? GetPercentile(quantifierResult.QuantifiedLinesAdded, Context.AdditionPercentile)
                : GetPercentile(quantifierResult.QuantifiedLinesDeleted, Context.DeletionPercentile);
        }

        private float GetFormulaPercentile(QuantifierResult quantifierResult)
        {
            var contextFormulaPercentile =
                Context.FormulaPercentile.First(f => f.Item1 == quantifierResult.Formula).Item2;
            return GetPercentile(quantifierResult.FormulaLinesChanged, contextFormulaPercentile);
        }

        private float GetPercentile(
            int finalValue,
            SortedDictionary<int, float> contextPercentile)
        {
            var operationValues = contextPercentile.Keys.ToArray();

            var idxUpperBound = Array.FindIndex(
                operationValues,
                arrayElement =>
                    finalValue <=
                    arrayElement);

            var idxLowerBound = Array.FindLastIndex(
                operationValues,
                arrayElement =>
                    arrayElement <= finalValue);

            var lowerBoundPercentile =
                finalValue <
                operationValues[0]
                    ? 0
                    : contextPercentile[operationValues[idxLowerBound]];

            var upperBoundPercentile =
                finalValue >
                operationValues[^1]
                    ? 100
                    : contextPercentile[operationValues[idxUpperBound]];

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
                ThresholdFormula.Avg => (quantifierResult.QuantifiedLinesAdded +
                                         quantifierResult.QuantifiedLinesDeleted) / 2,
                ThresholdFormula.Min => Math.Min(
                    quantifierResult.QuantifiedLinesAdded,
                    quantifierResult.QuantifiedLinesDeleted),
                ThresholdFormula.Max => Math.Max(
                    quantifierResult.QuantifiedLinesAdded,
                    quantifierResult.QuantifiedLinesDeleted),
                _ => 0
            };
        }
    }
}