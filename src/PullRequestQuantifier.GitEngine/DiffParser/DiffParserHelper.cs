namespace PullRequestQuantifier.GitEngine.DiffParser
{
    using System.Collections.Generic;
    using System.Linq;
    using PullRequestQuantifier.GitEngine.DiffParser.Models;

    public static class DiffParserHelper
    {
        public static IEnumerable<FileDiff> Parse(string input, string lineEnding = "\n")
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return Enumerable.Empty<FileDiff>();
            }

            IEnumerable<string> lines = StringHelper.SplitLines(input, lineEnding).ToList();

            if (!lines.Any())
            {
                return Enumerable.Empty<FileDiff>();
            }

            var parser = new DiffParser();

            return parser.Run(lines);
        }
    }
}
