namespace PrQuantifier.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PrQuantifier.Core.Git;

    // todo for some of the extensions we need language specific implementation for now just do it for c sharp
    // todo optimize this part to have only one iteration pass over the changes and lines, trim once ...
    public static class GitFilePatchExtensions
    {
        public static void RemoveWhiteSpacesChanges(this GitFilePatch gitFilePatch)
        {
            if (gitFilePatch == null)
            {
                throw new ArgumentNullException(nameof(gitFilePatch));
            }

            // first remove lines containing only white spaces
            gitFilePatch.DiffContentLines = gitFilePatch.DiffContentLines
                .Where(IsDiffLine)
                .Select(line => line.Trim())
                .Where(line => !line.Equals("+") && !line.Equals("-")).ToArray();
        }

        public static void RemoveCommentsChanges(this GitFilePatch gitFilePatch)
        {
            if (gitFilePatch == null)
            {
                throw new ArgumentNullException(nameof(gitFilePatch));
            }

            // todo later expand  to comments sections, and add language specific parsers
            gitFilePatch.DiffContentLines = gitFilePatch.DiffContentLines
                .Where(IsDiffLine)
                .Select(line => line.Trim())
                .Where(line => !IsLineComment(line)).ToArray();
        }

        public static void RemoveCodeBlockSeparatorChanges(this GitFilePatch gitFilePatch)
        {
            if (gitFilePatch == null)
            {
                throw new ArgumentNullException(nameof(gitFilePatch));
            }

            // todo later add language specific parsers
            gitFilePatch.DiffContentLines = gitFilePatch.DiffContentLines
                .Where(IsDiffLine)
                .Select(line => line.Trim())
                .Where(line => !IsLineCodeBlockSeparator(line)).ToArray();
        }

        public static void RemoveRenamedChanges(this GitFilePatch gitFilePatch)
        {
            if (gitFilePatch.ChangeType != GitChangeType.Renamed)
            {
                return;
            }

            gitFilePatch.DiscardFromCounting = true;
        }

        public static void RemoveCopiedChanges(this GitFilePatch gitFilePatch)
        {
            if (gitFilePatch.ChangeType != GitChangeType.Copied)
            {
                return;
            }

            gitFilePatch.DiscardFromCounting = true;
        }

        public static void ComputeChanges(this GitFilePatch gitFilePatch)
        {
            if (gitFilePatch == null)
            {
                throw new ArgumentNullException(nameof(gitFilePatch));
            }

            // if file change was discarded from the counted do nothing.
            if (gitFilePatch.DiscardFromCounting)
            {
                return;
            }

            // consider all lines that accounts for addition or deletion, exclude those with multiple ++ or --
            // example --- a/src/PrQuantifier/PrQuantifier.csproj
            // +++ b/src/PrQuantifier/PrQuantifier.csproj
            gitFilePatch.QuantifiedLinesAdded =
                gitFilePatch.DiffContentLines.Count(line => line.StartsWith('+') && !line.StartsWith("++"));
            gitFilePatch.QuantifiedLinesDeleted =
                gitFilePatch.DiffContentLines.Count(line => line.StartsWith('-') && !line.StartsWith("--"));
        }

        private static bool IsDiffLine(string line)
        {
            return (line.StartsWith('+') || line.StartsWith('-'))
                   && !line.StartsWith("++")
                   && !line.StartsWith("--");
        }

        private static bool IsLineComment(string line)
        {
            var simplifyLine = line.Replace("+", string.Empty).Replace("-", string.Empty).Trim();

            return simplifyLine.StartsWith("//") || simplifyLine.StartsWith("/*");
        }

        private static bool IsLineCodeBlockSeparator(string line)
        {
            var simplifyLine = line.Replace("+", string.Empty).Replace("-", string.Empty).Trim();

            return simplifyLine.StartsWith("{") || simplifyLine.Equals("{}") || simplifyLine.StartsWith("}");
        }
    }
}
