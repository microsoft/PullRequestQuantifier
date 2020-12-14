namespace PrQuantifier
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Extensions;
    using global::PrQuantifier.Core.Git;

    public sealed class PrQuantifier : IPrQuantifier
    {
        private readonly Context context;

        public PrQuantifier(Context context)
        {
            this.context = context;
        }

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

                // todo involve context and compute
                foreach (GitFilePatch quantifierInputChange in quantifierInput.Changes)
                {
                    ApplyLanguageContext(quantifierInputChange);
                }

                CountChanges(quantifierInput, quantifierResult);

                // compute the label using the context percentile information and the thresholds
                SetLabel(quantifierResult);
            });

            return quantifierResult;
        }

        private void ApplyLanguageContext(GitFilePatch quantifierInputChange)
        {
            // no language context found continue without
            if (context.LanguageOptions == null)
            {
                return;
            }

            if (context.LanguageOptions.IgnoreSpaces)
            {
                quantifierInputChange.RemoveWhiteSpacesChanges();
            }

            if (context.LanguageOptions.IgnoreComments)
            {
                quantifierInputChange.RemoveCommentsChanges();
            }

            if (context.LanguageOptions.IgnoreCodeBlockSeparator)
            {
                quantifierInputChange.RemoveCodeBlockSeparatorChanges();
            }
        }

        private void CountChanges(
            QuantifierInput quantifierInput,
            QuantifierResult quantifierResult)
        {
            // do the final count of changes
            foreach (GitFilePatch quantifierInputChange in quantifierInput.Changes)
            {
                // todo for now remove from counting renamed or copied files, expose this in the context
                quantifierInputChange.RemoveRenamedChanges();
                quantifierInputChange.RemoveCopiedChanges();
                quantifierInputChange.ComputeChanges();
            }

            quantifierResult.QuantifiedLinesAdded = quantifierInput.Changes.Sum(c => c.QuantifiedLinesAdded);
            quantifierResult.QuantifiedLinesDeleted = quantifierInput.Changes.Sum(c => c.QuantifiedLinesDeleted);
        }

        private void SetLabel(QuantifierResult quantifierResult)
        {
            // in case no addition/deletion percentile found  then we won't be able to set the label.
            if (context.AdditionPercentile == null
                || context.DeletionPercentile == null
                || context.AdditionPercentile.Count == 0
                || context.DeletionPercentile.Count == 0)
            {
                return;
            }

            var additionPercentile = GetPercentile(quantifierResult, true);
            var deletionPercentile = GetPercentile(quantifierResult, false);

            // todo come up with a better way combine addition and deletion, maybe use weights
            var avgPercentile = (additionPercentile + deletionPercentile) / 2;

            foreach (var contextThreshold in context.Thresholds.OrderBy(t => t.Value))
            {
                // we set the label from the thresholds and exit when we have first value threshold grater then percentile
                quantifierResult.Label = contextThreshold.Label;
                if (contextThreshold.Value >= avgPercentile)
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
                ? context.AdditionPercentile.Keys.ToArray()
                : context.DeletionPercentile.Keys.ToArray();

            var idxUpperBound = Array.FindIndex(
                operationValues,
                arrayElement =>
                    (addition ? quantifierResult.QuantifiedLinesAdded : quantifierResult.QuantifiedLinesDeleted) <=
                    arrayElement);

            var idxLowerBound = Array.FindIndex(
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
                        ? context.AdditionPercentile[operationValues[idxLowerBound]]
                        : context.DeletionPercentile[operationValues[idxLowerBound]]);

            var upperBoundPercentile =
                (addition ? quantifierResult.QuantifiedLinesAdded : quantifierResult.QuantifiedLinesDeleted) >
                operationValues[^1]
                    ? 100
                    : (addition
                        ? context.AdditionPercentile[operationValues[idxUpperBound]]
                        : context.DeletionPercentile[operationValues[idxUpperBound]]);

            // todo here change this and compute accurately
            return Math.Max(lowerBoundPercentile, upperBoundPercentile);
        }
    }
}