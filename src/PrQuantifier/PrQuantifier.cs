namespace PrQuantifier
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Extensions;
    using global::PrQuantifier.Core.Git;

    public class PrQuantifier : IPrQuantifier
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
    }
}