namespace PullRequestQuantifier.Abstractions.Git.DiffParser.Models
{
    internal class LineDiff
    {
        public LineDiff(LineChangeType type, int index, string content)
        {
            Type = type;
            Index = index;
            Content = content;
        }

        public LineDiff(int oldIndex, int newIndex, string content)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
            Type = LineChangeType.Normal;
            Content = content;
        }

        public bool Add => Type == LineChangeType.Add;

        public bool Delete => Type == LineChangeType.Delete;

        public bool Normal => Type == LineChangeType.Normal;

        public string Content { get; }

        public int Index { get; }

        public int OldIndex { get; }

        public int NewIndex { get; }

        public LineChangeType Type { get; }
    }
}
