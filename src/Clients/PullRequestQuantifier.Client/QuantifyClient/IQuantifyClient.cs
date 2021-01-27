namespace PullRequestQuantifier.Client.QuantifyClient
{
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;

    public interface IQuantifyClient
    {
        /// <summary>
        /// Gets quantifier context.
        /// </summary>
        Context Context { get; }

        /// <summary>
        /// Compute using git local evaluation.
        /// </summary>
        /// <param name="gitRepoPath">Path to the git repository.</param>
        /// <returns>returns an evaluation result.</returns>
        Task<QuantifierResult> Compute(string gitRepoPath);

        /// <summary>
        /// Quantify a pull request against a commit.
        /// </summary>
        /// <param name="gitRepoPath">Path to the git repository.</param>
        /// <param name="commitSha1">Sha1 of the commit (example 78d1ffd2b6fbcdf2b5f278df0c22bb09a0971c0b).</param>
        /// <returns>returns an evaluation result.</returns>
        Task<QuantifierResult> Compute(
            string gitRepoPath,
            string commitSha1);

        /// <summary>
        /// Compute using a quantifier input from outside.
        /// </summary>
        /// <param name="quantifierInput">The quantifier input.</param>
        /// <returns>returns an evaluation result.</returns>
        Task<QuantifierResult> Compute(QuantifierInput quantifierInput);
    }
}