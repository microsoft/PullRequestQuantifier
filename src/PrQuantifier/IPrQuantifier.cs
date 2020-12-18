namespace PrQuantifier
{
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Context;

    public interface IPrQuantifier
    {
        /// <summary>
        /// Gets quantifier context.
        /// </summary>
        Context Context { get; }

        /// <summary>
        /// Quantifies based on the input.
        /// </summary>
        /// <param name="quantifierInput">Input quantifier.</param>
        /// <returns>returns a quantifier result.</returns>
        Task<QuantifierResult> Quantify(QuantifierInput quantifierInput);
    }
}
