namespace PullRequestQuantifier.Client.QuantifyClient
***REMOVED***
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;

    public interface IQuantifyClient
    ***REMOVED***
        /// <summary>
        /// Gets quantifier context.
        /// </summary>
        Context Context ***REMOVED*** get; ***REMOVED***

        /// <summary>
        /// Compute using git local evaluation.
        /// </summary>
        /// <param name="gitRepoPath">Path to the git repository.</param>
        /// <returns>returns an evaluation result.</returns>
        Task<QuantifierClientResult> Compute(string gitRepoPath);

        /// <summary>
        /// Compute using a quantifier input from outside.
        /// </summary>
        /// <param name="quantifierInput">The quantifier input.</param>
        /// <returns>returns an evaluation result.</returns>
        Task<QuantifierClientResult> Compute(QuantifierInput quantifierInput);
***REMOVED***
***REMOVED***