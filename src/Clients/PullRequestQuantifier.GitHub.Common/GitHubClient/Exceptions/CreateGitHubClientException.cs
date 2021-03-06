namespace PullRequestQuantifier.GitHub.Common.GitHubClient.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
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
