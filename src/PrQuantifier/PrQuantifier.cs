namespace PrQuantifier
***REMOVED***
    using System;
    using System.Linq;
    using global::PrQuantifier.Core.Context;

    public class PrQuantifier : IPrQuantifier
    ***REMOVED***
        private readonly Context context;

        public PrQuantifier(Context context)
        ***REMOVED***
            this.context = context;
***REMOVED***

        /// <inheritdoc />
        public QuantifierResult Quantify(QuantifierInput quantifierInput)
        ***REMOVED***
            if (quantifierInput == null)
            ***REMOVED***
                throw new ArgumentNullException(nameof(quantifierInput));
    ***REMOVED***

            // todo execute quantifier for this context and this particular input
            return Compute(quantifierInput);
***REMOVED***

        private QuantifierResult Compute(QuantifierInput quantifierInput)
        ***REMOVED***
            var quantifierResult = new QuantifierResult
            ***REMOVED***
                QuantifierInput = quantifierInput,
                QuantifiedLinesAdded = quantifierInput.Changes.Sum(c => c.AbsoluteLinesAdded),
                QuantifiedLinesDeleted = quantifierInput.Changes.Sum(c => c.AbsoluteLinesDeleted)
    ***REMOVED***;

            // todo involve context and compute
            return quantifierResult;
***REMOVED***
***REMOVED***
***REMOVED***