namespace PullRequestQuantifier.GitHub.Common.Events
{
    using System.Threading.Tasks;
    using PullRequestQuantifier.GitHub.Common.Models;

    public interface IGitHubEventHandler
    {
        GitHubEventActions EventType { get; }

        Task HandleEvent(string gitHubEvent);
    }
}
