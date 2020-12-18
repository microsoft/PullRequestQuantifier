namespace PrQuantifier.Core.Extensions
***REMOVED***
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PrQuantifier.Core.Git;

    // todo for some of the extensions we need language specific implementation for now just do it for c sharp
    // todo optimize this part to have only one iteration pass over the changes and lines, trim once ...
    public static class GitFilePatchExtensions
    ***REMOVED***
        public static void RemoveWhiteSpacesChanges(this GitFilePatch gitFilePatch)
        ***REMOVED***
            if (gitFilePatch == null)
            ***REMOVED***
                throw new ArgumentNullException(nameof(gitFilePatch));
    ***REMOVED***

            // first remove lines containing only white spaces
            gitFilePatch.DiffContentLines = gitFilePatch.DiffContentLines
                .Where(IsDiffLine)
                .Select(line => line.Trim())
                .Where(line => !line.Equals("+") && !line.Equals("-")).ToArray();
***REMOVED***

        public static void RemoveCommentsChanges(this GitFilePatch gitFilePatch)
        ***REMOVED***
            if (gitFilePatch == null)
            ***REMOVED***
                throw new ArgumentNullException(nameof(gitFilePatch));
    ***REMOVED***

            // todo later expand  to comments sections, and add language specific parsers
            gitFilePatch.DiffContentLines = gitFilePatch.DiffContentLines
                .Where(IsDiffLine)
                .Select(line => line.Trim())
                .Where(line => !IsLineComment(line)).ToArray();
***REMOVED***

        public static void RemoveCodeBlockSeparatorChanges(this GitFilePatch gitFilePatch)
        ***REMOVED***
            if (gitFilePatch == null)
            ***REMOVED***
                throw new ArgumentNullException(nameof(gitFilePatch));
    ***REMOVED***

            // todo later add language specific parsers
            gitFilePatch.DiffContentLines = gitFilePatch.DiffContentLines
                .Where(IsDiffLine)
                .Select(line => line.Trim())
                .Where(line => !IsLineCodeBlockSeparator(line)).ToArray();
***REMOVED***

        public static void RemoveRenamedChanges(this GitFilePatch gitFilePatch)
        ***REMOVED***
            if (gitFilePatch.ChangeType != GitChangeType.Renamed)
            ***REMOVED***
                return;
    ***REMOVED***

            gitFilePatch.DiffContentLines = new List<string>().ToArray();
***REMOVED***

        public static void RemoveCopiedChanges(this GitFilePatch gitFilePatch)
        ***REMOVED***
            if (gitFilePatch.ChangeType != GitChangeType.Copied)
            ***REMOVED***
                return;
    ***REMOVED***

            gitFilePatch.DiffContentLines = new List<string>().ToArray();
***REMOVED***

        public static void ComputeChanges(this GitFilePatch gitFilePatch)
        ***REMOVED***
            if (gitFilePatch == null)
            ***REMOVED***
                throw new ArgumentNullException(nameof(gitFilePatch));
    ***REMOVED***

            // consider all lines that accounts for addition or deletion, exclude those with multiple ++ or --
            // example --- a/src/PrQuantifier/PrQuantifier.csproj
            // +++ b/src/PrQuantifier/PrQuantifier.csproj
            gitFilePatch.QuantifiedLinesAdded =
                gitFilePatch.DiffContentLines.Count(line => line.StartsWith('+') && !line.StartsWith("++"));
            gitFilePatch.QuantifiedLinesDeleted =
                gitFilePatch.DiffContentLines.Count(line => line.StartsWith('-') && !line.StartsWith("--"));
***REMOVED***

        private static bool IsDiffLine(string line)
        ***REMOVED***
            return (line.StartsWith('+') || line.StartsWith('-'))
                   && !line.StartsWith("++")
                   && !line.StartsWith("--");
***REMOVED***

        private static bool IsLineComment(string line)
        ***REMOVED***
            var simplifyLine = line.Replace("+", string.Empty).Replace("-", string.Empty).Trim();

            return simplifyLine.StartsWith("//") || simplifyLine.StartsWith("/*");
***REMOVED***

        private static bool IsLineCodeBlockSeparator(string line)
        ***REMOVED***
            var simplifyLine = line.Replace("+", string.Empty).Replace("-", string.Empty).Trim();

            return simplifyLine.StartsWith("***REMOVED***") || simplifyLine.Equals("***REMOVED******REMOVED***") || simplifyLine.StartsWith("***REMOVED***");
***REMOVED***
***REMOVED***
***REMOVED***
