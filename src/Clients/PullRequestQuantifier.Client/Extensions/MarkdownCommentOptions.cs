namespace PullRequestQuantifier.Client.Extensions
{
    public sealed class MarkdownCommentOptions
    {
        public MarkdownCommentOptions()
        {
            CollapsePullRequestQuantifiedSection = true;
        }

        public bool CollapsePullRequestQuantifiedSection { get; set; }
    }
}
