namespace PrQuantifier.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PrQuantifier.Core.Git;

    // todo for some of the extensions we need language specific implementation for now just do it for c sharp
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
                .Where(line => line.StartsWith('+') || line.StartsWith('-'))
                .Where(line => !line.StartsWith("++") || !line.StartsWith("--"))
                .Select(line => line.Trim())
                .Where(line => !line.Equals("+") || !line.Equals("-")).ToArray();
        }

        public static void RemoveCommentsChanges(this GitFilePatch gitFilePatch)
        {
            // todo
        }

        public static void RemoveCodeBlockSeparatorChanges(this GitFilePatch gitFilePatch)
        {
            // todo
        }

        public static void RemoveRenamedChanges(this GitFilePatch gitFilePatch)
        {
            if (gitFilePatch.ChangeType != GitChangeType.Renamed)
            {
                return;
            }

            gitFilePatch.DiffContentLines = new List<string>().ToArray();
        }

        public static void RemoveCopiedChanges(this GitFilePatch gitFilePatch)
        {
            if (gitFilePatch.ChangeType != GitChangeType.Copied)
            {
                return;
            }

            gitFilePatch.DiffContentLines = new List<string>().ToArray();
        }

        public static void ComputeChanges(this GitFilePatch gitFilePatch)
        {
            if (gitFilePatch == null)
            {
                throw new ArgumentNullException(nameof(gitFilePatch));
            }

            // consider all lines that accounts for addition or deletion, exclude those with multiple ++ or --
            // example --- a/src/PrQuantifier/PrQuantifier.csproj
            // +++ b/src/PrQuantifier/PrQuantifier.csproj
            gitFilePatch.QuantifiedLinesAdded =
                gitFilePatch.DiffContentLines.Count(line => line.StartsWith('+') && !line.StartsWith("++"));
            gitFilePatch.QuantifiedLinesDeleted =
                gitFilePatch.DiffContentLines.Count(line => line.StartsWith('-') && !line.StartsWith("--"));
        }
    }
}
