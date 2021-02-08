namespace PullRequestQuantifier.GitHub.Client.Events
{
    using System.Threading.Tasks;
    using PullRequestQuantifier.GitHub.Client.Models;

    public interface IGitHubEventHandler
    {
        GitHubEventActions EventType { get; }

        Task HandleEvent(string gitHubEvent);
    }
}
