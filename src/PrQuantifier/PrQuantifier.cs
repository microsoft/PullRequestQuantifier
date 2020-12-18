namespace PrQuantifier
***REMOVED***
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Extensions;
    using global::PrQuantifier.Core.Git;

    public sealed class PrQuantifier : IPrQuantifier
    ***REMOVED***
        public PrQuantifier(Context context)
        ***REMOVED***
            Context = context;
***REMOVED***

        public Context Context ***REMOVED*** get; ***REMOVED***

        /// <inheritdoc />
        public async Task<QuantifierResult> Quantify(QuantifierInput quantifierInput)
        ***REMOVED***
            if (quantifierInput == null)
            ***REMOVED***
                throw new ArgumentNullException(nameof(quantifierInput));
    ***REMOVED***

            // execute quantifier for this context and this particular input
            return await Compute(quantifierInput);
***REMOVED***

        private async Task<QuantifierResult> Compute(QuantifierInput quantifierInput)
        ***REMOVED***
            QuantifierResult quantifierResult = null;

            await Task.Factory.StartNew(() =>
            ***REMOVED***
                quantifierResult = new QuantifierResult
                ***REMOVED***
                    QuantifierInput = quantifierInput
        ***REMOVED***;

                // involve context and compute
                foreach (GitFilePatch quantifierInputChange in quantifierInput.Changes)
                ***REMOVED***
                    ApplyContext(quantifierInputChange);
        ***REMOVED***

                CountTotalChanges(quantifierInput, quantifierResult);

                // compute the label using the context percentile information and the thresholds
                SetLabel(quantifierResult);
    ***REMOVED***);

            return quantifierResult;
***REMOVED***

        private void ApplyContext(GitFilePatch quantifierInputChange)
        ***REMOVED***
            // no language context found continue without
            if (Context.LanguageOptions != null)
            ***REMOVED***
                if (Context.LanguageOptions.IgnoreSpaces)
                ***REMOVED***
                    quantifierInputChange.RemoveWhiteSpacesChanges();
        ***REMOVED***

                if (Context.LanguageOptions.IgnoreComments)
                ***REMOVED***
                    quantifierInputChange.RemoveCommentsChanges();
        ***REMOVED***

                if (Context.LanguageOptions.IgnoreCodeBlockSeparator)
                ***REMOVED***
                    quantifierInputChange.RemoveCodeBlockSeparatorChanges();
        ***REMOVED***
    ***REMOVED***

            quantifierInputChange.RemoveRenamedChanges();
            quantifierInputChange.RemoveCopiedChanges();

            // do the final count of changes
            quantifierInputChange.ComputeChanges();
***REMOVED***

        private void CountTotalChanges(
            QuantifierInput quantifierInput,
            QuantifierResult quantifierResult)
        ***REMOVED***
            quantifierResult.QuantifiedLinesAdded = quantifierInput.Changes.Sum(c => c.QuantifiedLinesAdded);
            quantifierResult.QuantifiedLinesDeleted = quantifierInput.Changes.Sum(c => c.QuantifiedLinesDeleted);
***REMOVED***

        private void SetLabel(QuantifierResult quantifierResult)
        ***REMOVED***
            // in case no addition/deletion found then we won't be able to set the label.
            if (Context.AdditionPercentile == null
                || Context.DeletionPercentile == null
                || Context.AdditionPercentile.Count == 0
                || Context.DeletionPercentile.Count == 0)
            ***REMOVED***
                return;
    ***REMOVED***

            quantifierResult.PercentileAddition = MathF.Round(GetPercentile(quantifierResult, true), 2);
            quantifierResult.PercentileDeletion = MathF.Round(GetPercentile(quantifierResult, false), 2);

            if (quantifierResult.QuantifiedLinesDeleted == 0 && quantifierResult.QuantifiedLinesAdded == 0)
            ***REMOVED***
                quantifierResult.Label = "No Changes";
                return;
    ***REMOVED***

            // todo come up with a better way combine addition and deletion, maybe use weights
            // for now to set the label use the absolute values addition/deletion and compare them with the thresholds
            // the percentile will be displayed saying that if your change has this number
            // of lines additions then you are at this percentile within this context
            // consider the sum for addition and deletion
            var changeNumber = quantifierResult.QuantifiedLinesAdded + quantifierResult.QuantifiedLinesDeleted;
            foreach (var contextThreshold in Context.Thresholds.OrderBy(t => t.Value))
            ***REMOVED***
                // we set the label from the thresholds and exit when we have first value threshold grater then percentile
                quantifierResult.Label = contextThreshold.Label;
                if (changeNumber <= contextThreshold.Value)
                ***REMOVED***
                    return;
        ***REMOVED***
    ***REMOVED***
***REMOVED***

        private float GetPercentile(
            QuantifierResult quantifierResult,
            bool addition)
        ***REMOVED***
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
***REMOVED***
***REMOVED***
***REMOVED***