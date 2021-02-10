namespace PullRequestQuantifier.GitHub.Client.Models
{
    using Newtonsoft.Json;
    using Octokit;

    /// <summary>
    /// This model has to be created by us as it is not present in
    /// Octokit.NET library.
    /// </summary>
    public class InstallationRepositoriesEventPayload
    {
        public string Action { get; protected set; }

        public User Sender { get; protected set; }

        public Installation Installation { get; protected set; }

        [JsonProperty("repositories_removed")]
        public Repository[] RepositoriesRemoved { get; protected set; }

        [JsonProperty("repositories_added")]
        public Repository[] RepositoriesAdded { get; protected set; }

        [JsonProperty("repository_selection")]
        public string RepositorySelection { get; protected set; }
    }
}
