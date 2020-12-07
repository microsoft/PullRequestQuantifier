namespace PrQuantifier
{
    using System;
    using System.Linq;
    using global::PrQuantifier.Core.Context;

    public class PrQuantifier : IPrQuantifier
    {
        private readonly Context context;

        public PrQuantifier(Context context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public QuantifierResult Quantify(QuantifierInput quantifierInput)
        {
            if (quantifierInput == null)
            {
                throw new ArgumentNullException(nameof(quantifierInput));
            }

            // todo execute quantifier for this context and this particular input
            return Compute(quantifierInput);
        }

        private QuantifierResult Compute(QuantifierInput quantifierInput)
        {
            var quantifierResult = new QuantifierResult
            {
                QuantifierInput = quantifierInput,
                QuantifiedLinesAdded = quantifierInput.Changes.Sum(c => c.AbsoluteLinesAdded),
                QuantifiedLinesDeleted = quantifierInput.Changes.Sum(c => c.AbsoluteLinesDeleted)
            };

            // todo involve context and compute
            return quantifierResult;
        }
    }
}