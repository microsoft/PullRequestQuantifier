namespace PullRequestQuantifier.GitHub.Common.Models
{
    using Octokit;

    /// <summary>
    /// This model has to be created by us as it is not present in
    /// Octokit.NET library.
    /// </summary>
    public class InstallationEventPayload
    {
        public string Action { get; protected set; }

        public User Sender { get; protected set; }

        public Installation Installation { get; protected set; }

        public Repository[] Repositories { get; protected set; }
    }
}
