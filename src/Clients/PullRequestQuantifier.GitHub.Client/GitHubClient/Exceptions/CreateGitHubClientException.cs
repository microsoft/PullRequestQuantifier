namespace PullRequestQuantifier.GitHub.Client.GitHubClient.Exceptions
{
    using System;

    public class CreateGitHubClientException : Exception
    {
        public CreateGitHubClientException()
        {
        }

        public CreateGitHubClientException(string message)
            : base(message)
        {
        }

        public CreateGitHubClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
