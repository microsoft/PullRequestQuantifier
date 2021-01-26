namespace PullRequestQuantifier.Abstractions.Git
{
    using System.Collections.Generic;
    using System.Linq;
    using Ignore;
    using PullRequestQuantifier.Abstractions.Git.DiffParser.Models;
    using PullRequestQuantifier.Common;

    // todo for some of the extensions we need language specific implementation for now just do it for c sharp
    // todo optimize this part to have only one iteration pass over the changes and lines, trim once ...
    public static class GitFilePatchExtensions
    {
        /// <summary>
        /// Discard from counting if the file should be excluded.
        /// Handles both <see cref="Context.Context.Included"/> and <see cref="Context.Context.Excluded"/>.
        /// </summary>
        /// <param name="gitFilePatch">Git file patch.</param>
        /// <param name="includedPatterns">Gitignore style patterns to include.</param>
        /// <param name="excludedPatterns">Gitignore style patterns to exclude.</param>
        /// <returns>True if the file patch is excluded, false otherwise.</returns>
        public static bool RemoveNotIncludedOrExcluded(
            this GitFilePatch gitFilePatch,
            IEnumerable<string> includedPatterns,
            IEnumerable<string> excludedPatterns)
        {
            ArgumentCheck.ParameterIsNotNull(gitFilePatch, nameof(gitFilePatch));

            var ignore = new Ignore();

            // if included patterns are specified, do not consider excluded patterns
            var includedPatternList = includedPatterns?.ToList();
            if (includedPatternList != null && includedPatternList.Any())
            {
                ignore.Add(includedPatternList);

                // not ignored by ignore effectively means this is not a match
                // and must be excluded
                if (!ignore.IsIgnored(gitFilePatch.FilePath))
                {
                    gitFilePatch.DiscardFromCounting = true;
                    return true;
                }

                return false;
            }

            var excludedPatternList = excludedPatterns?.ToList();
            if (excludedPatternList == null)
            {
                return false;
            }

            ignore.Add(excludedPatternList);
            if (ignore.IsIgnored(gitFilePatch.FilePath))
            {
                gitFilePatch.DiscardFromCounting = true;
                return true;
            }

            return false;
        }

        public static void RemoveWhiteSpacesChanges(this GitFilePatch gitFilePatch)
        {
            ArgumentCheck.ParameterIsNotNull(gitFilePatch, nameof(gitFilePatch));

            // first remove lines containing only white spaces
            gitFilePatch.DiffLines = gitFilePatch.DiffLines
                .Where(l => l.Type != LineChangeType.Normal)
                .Where(l => !string.IsNullOrEmpty(l.Content.Trim()));
        }

        public static void RemoveCommentsChanges(this GitFilePatch gitFilePatch)
        {
            ArgumentCheck.ParameterIsNotNull(gitFilePatch, nameof(gitFilePatch));

            // todo later expand  to comments sections, and add language specific parsers
            gitFilePatch.DiffLines = gitFilePatch.DiffLines
                .Where(l => l.Type != LineChangeType.Normal)
                .Where(l => !IsLineComment(l.Content.Trim()));
        }

        public static void RemoveCodeBlockSeparatorChanges(this GitFilePatch gitFilePatch)
        {
            ArgumentCheck.ParameterIsNotNull(gitFilePatch, nameof(gitFilePatch));

            // todo later add language specific parsers
            gitFilePatch.DiffLines = gitFilePatch.DiffLines
                .Where(l => l.Type != LineChangeType.Normal)
                .Where(l => !IsLineCodeBlockSeparator(l.Content.Trim()));
        }

        public static void RemoveRenamedChanges(this GitFilePatch gitFilePatch)
        {
            ArgumentCheck.ParameterIsNotNull(gitFilePatch, nameof(gitFilePatch));

            if (gitFilePatch.ChangeType != GitChangeType.Renamed)
            {
                return;
            }

            gitFilePatch.DiscardFromCounting = true;
        }

        public static void RemoveCopiedChanges(this GitFilePatch gitFilePatch)
        {
            ArgumentCheck.ParameterIsNotNull(gitFilePatch, nameof(gitFilePatch));

            if (gitFilePatch.ChangeType != GitChangeType.Copied)
            {
                return;
            }

            gitFilePatch.DiscardFromCounting = true;
        }

        public static void ComputeChanges(this GitFilePatch gitFilePatch)
        {
            ArgumentCheck.ParameterIsNotNull(gitFilePatch, nameof(gitFilePatch));

            // if file change was discarded from the counted do nothing.
            if (gitFilePatch.DiscardFromCounting)
            {
                return;
            }

            gitFilePatch.QuantifiedLinesAdded = gitFilePatch.DiffLines.Count(l => l.Type == LineChangeType.Add);
            gitFilePatch.QuantifiedLinesDeleted = gitFilePatch.DiffLines.Count(l => l.Type == LineChangeType.Delete);
        }

        private static bool IsLineComment(string line)
        {
            line = line.Trim();
            return line.StartsWith("//") || line.StartsWith("/*");
        }

        private static bool IsLineCodeBlockSeparator(string line)
        {
            line = line.Trim();

            return line.StartsWith("{") || line.Equals("{}") || line.StartsWith("}");
        }
    }
}
