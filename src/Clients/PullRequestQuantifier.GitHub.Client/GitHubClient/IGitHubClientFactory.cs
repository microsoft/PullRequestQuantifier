namespace PullRequestQuantifier.GitHub.Client.GitHubClient
***REMOVED***
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the contract for creating a GitHub client.
    /// </summary>
    public interface IGitHubClientFactory
    ***REMOVED***
        /// <summary>Gets the GitHub client wrapper.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IGitHubClientAdapter> GetGitHubGitClientAsync();
***REMOVED***
***REMOVED***
