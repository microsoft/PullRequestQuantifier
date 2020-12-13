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
        private readonly Context context;

        public PrQuantifier(Context context)
        ***REMOVED***
            this.context = context;
***REMOVED***

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

                // todo involve context and compute
                foreach (GitFilePatch quantifierInputChange in quantifierInput.Changes)
                ***REMOVED***
                    ApplyLanguageContext(quantifierInputChange);
        ***REMOVED***

                CountChanges(quantifierInput, quantifierResult);
    ***REMOVED***);

            return quantifierResult;
***REMOVED***

        private void ApplyLanguageContext(GitFilePatch quantifierInputChange)
        ***REMOVED***
            // no language context found continue without
            if (context.LanguageOptions == null)
            ***REMOVED***
                return;
    ***REMOVED***

            if (context.LanguageOptions.IgnoreSpaces)
            ***REMOVED***
                quantifierInputChange.RemoveWhiteSpacesChanges();
    ***REMOVED***

            if (context.LanguageOptions.IgnoreComments)
            ***REMOVED***
                quantifierInputChange.RemoveCommentsChanges();
    ***REMOVED***

            if (context.LanguageOptions.IgnoreCodeBlockSeparator)
            ***REMOVED***
                quantifierInputChange.RemoveCodeBlockSeparatorChanges();
    ***REMOVED***
***REMOVED***

        private void CountChanges(
            QuantifierInput quantifierInput,
            QuantifierResult quantifierResult)
        ***REMOVED***
            // do the final count of changes
            foreach (GitFilePatch quantifierInputChange in quantifierInput.Changes)
            ***REMOVED***
                // todo for now remove from counting renamed or copied files, expose this in the context
                quantifierInputChange.RemoveRenamedChanges();
                quantifierInputChange.RemoveCopiedChanges();
                quantifierInputChange.ComputeChanges();
    ***REMOVED***

            quantifierResult.QuantifiedLinesAdded = quantifierInput.Changes.Sum(c => c.QuantifiedLinesAdded);
            quantifierResult.QuantifiedLinesDeleted = quantifierInput.Changes.Sum(c => c.QuantifiedLinesDeleted);
***REMOVED***
***REMOVED***
***REMOVED***