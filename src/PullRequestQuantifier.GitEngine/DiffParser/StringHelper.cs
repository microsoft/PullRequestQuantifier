namespace PullRequestQuantifier.GitEngine.DiffParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class StringHelper
    {
        public static IEnumerable<string> SplitLines(string input, string lineEnding)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return Enumerable.Empty<string>();
            }

            string[] lines = input.Split(new[] { lineEnding }, StringSplitOptions.None);
            return lines.Length == 0 ? Enumerable.Empty<string>() : lines;
        }
    }
}
