namespace PullRequestQuantifier.Client.ContextGenerator
***REMOVED***
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;

    public interface IContextGenerator
    ***REMOVED***
        /// <summary>
        /// Generate a context by given a repo path.
        /// </summary>
        /// <param name="repoPath">The repo  path.</param>
        /// <returns>returns a context.</returns>
        Task<Context> Create(string repoPath);
***REMOVED***
***REMOVED***
