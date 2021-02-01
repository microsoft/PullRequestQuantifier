namespace PullRequestQuantifier.Client.Extensions
{
    public sealed class MarkdownCommentOptions
    {
        public MarkdownCommentOptions()
        {
            CollapsePullRequestQuantifiedSection = true;
            CollapseChangesSummarySection = true;
        }

        public bool CollapsePullRequestQuantifiedSection { get; set; }

        public bool CollapseChangesSummarySection { get; set; }
    }
}
