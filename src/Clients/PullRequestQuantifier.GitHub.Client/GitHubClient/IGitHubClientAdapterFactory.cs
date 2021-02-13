namespace PullRequestQuantifier.GitHub.Client.GitHubClient
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the contract for creating a GitHub client.
    /// </summary>
    public interface IGitHubClientAdapterFactory
    {
        /// <summary>Gets the GitHub client adapter.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IGitHubClientAdapter> GetGitHubClientAdapterAsync(
            long installationId,
            string dnsHost);
    }
}
