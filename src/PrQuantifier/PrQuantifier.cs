namespace PrQuantifier
***REMOVED***
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Context;

    public class PrQuantifier : IPrQuantifier
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

            // todo execute quantifier for this context and this particular input
            return await Compute(quantifierInput);
***REMOVED***

        private async Task<QuantifierResult> Compute(QuantifierInput quantifierInput)
        ***REMOVED***
            QuantifierResult quantifierResult = null;

            await Task.Factory.StartNew(() =>
            ***REMOVED***
                quantifierResult = new QuantifierResult
                ***REMOVED***
                    QuantifierInput = quantifierInput,
                    QuantifiedLinesAdded = quantifierInput.Changes.Sum(c => c.AbsoluteLinesAdded),
                    QuantifiedLinesDeleted = quantifierInput.Changes.Sum(c => c.AbsoluteLinesDeleted)
        ***REMOVED***;

                // todo involve context and compute
    ***REMOVED***);

            return quantifierResult;
***REMOVED***
***REMOVED***
***REMOVED***