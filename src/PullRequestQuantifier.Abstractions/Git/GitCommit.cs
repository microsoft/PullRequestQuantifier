namespace PullRequestQuantifier.Abstractions.Git
{
    using System;

    public sealed class GitCommit
    {
        public string AuthorName { get; set; }

        public string Title { get; set; }

        public string Sha { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }
    }
}
