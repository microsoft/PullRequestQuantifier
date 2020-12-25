namespace PrQuantifier.Client
***REMOVED***
    using System.Threading.Tasks;
    using global::PrQuantifier.Abstractions.Context;
    using global::PrQuantifier.Abstractions.Core;

    public interface IQuantifyClient
    ***REMOVED***
        /// <summary>
        /// Gets quantifier context.
        /// </summary>
        Context Context ***REMOVED*** get; ***REMOVED***

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
***REMOVED***
***REMOVED***