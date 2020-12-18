namespace PrQuantifier.Client
{
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Abstractions;

    public interface IQuantifyClient
    {
        /// <summary>
        /// Gets a git engine for local evaluation.
        /// </summary>
        public IGitEngine GitEngine { get; }

        /// <summary>
        /// Compute using git local evaluation.
        /// </summary>
        /// <returns>returns an evaluation result.</returns>
        Task<QuantifierResult> Compute();

        /// <summary>
        /// Compute using a quantifier input from outside.
        /// </summary>
        /// <param name="quantifierInput">The quantifier input.</param>
        /// <returns>returns an evaluation result.</returns>
        Task<QuantifierResult> Compute(QuantifierInput quantifierInput);
    }
}
