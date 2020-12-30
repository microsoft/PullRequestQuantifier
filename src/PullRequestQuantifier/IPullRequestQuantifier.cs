namespace PullRequestQuantifier
{
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;

    public interface IPullRequestQuantifier
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
