namespace PullRequestQuantifier.Abstractions.Git.DiffParser.Models
{
    using System.Collections.Generic;

    internal class Chunk
    {
        public Chunk(string content, ChunkRangeInfo rangeInfo)
        {
            Content = content;
            RangeInfo = rangeInfo;
            Changes = new List<LineDiff>();
        }

        public ICollection<LineDiff> Changes { get; }

        public string Content { get; }

        public ChunkRangeInfo RangeInfo { get; }
    }
}
