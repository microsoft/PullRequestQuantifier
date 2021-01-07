namespace PullRequestQuantifier.GitHub.Client.GitHubClient
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the contract for creating a GitHub client.
    /// </summary>
    public interface IGitHubClientFactory
    {
        /// <summary>Gets the GitHub client wrapper.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IGitHubClientAdapter> GetGitHubGitClientAsync();
    }
}
