namespace PullRequestQuantifier.GitEngine.DiffParser.Models
{
    using System.Collections.Generic;

    public class Chunk
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
