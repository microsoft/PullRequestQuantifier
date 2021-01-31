namespace PullRequestQuantifier.Abstractions.Git.DiffParser
{
    using System.Collections.Generic;
    using System.Linq;
    using PullRequestQuantifier.Abstractions.Git.DiffParser.Models;

    internal static class PatchHelper
    {
        public static string Patch(string src, IEnumerable<Chunk> chunks, string lineEnding)
        {
            IEnumerable<string> srcLines = StringHelper.SplitLines(src, lineEnding);
            IList<string> dstLines = new List<string>(srcLines);

            foreach (Chunk chunk in chunks)
            {
                int lineIndex = 0;

                if (chunk.RangeInfo.NewRange.StartLine != 0)
                {
                    lineIndex = chunk.RangeInfo.NewRange.StartLine - 1; // zero-index the start line
                }

                foreach (LineDiff lineDiff in chunk.Changes)
                {
                    if (lineDiff.Add)
                    {
                        dstLines.Insert(lineIndex, lineDiff.Content);
                        lineIndex++;
                    }
                    else if (lineDiff.Delete)
                    {
                        dstLines.RemoveAt(lineIndex);
                    }
                    else if (lineDiff.Normal)
                    {
                        lineIndex++;
                    }
                }
            }

            string patchString = string.Join(lineEnding, dstLines.ToArray());
            return patchString;
        }
    }
}
