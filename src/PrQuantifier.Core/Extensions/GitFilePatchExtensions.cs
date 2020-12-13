namespace PrQuantifier.Core.Extensions
***REMOVED***
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PrQuantifier.Core.Git;

    // todo for some of the extensions we need language specific implementation for now just do it for c sharp
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
                .Where(line => line.StartsWith('+') || line.StartsWith('-'))
                .Where(line => !line.StartsWith("++") || !line.StartsWith("--"))
                .Select(line => line.Trim())
                .Where(line => !line.Equals("+") || !line.Equals("-")).ToArray();
***REMOVED***

        public static void RemoveCommentsChanges(this GitFilePatch gitFilePatch)
        ***REMOVED***
            // todo
***REMOVED***

        public static void RemoveCodeBlockSeparatorChanges(this GitFilePatch gitFilePatch)
        ***REMOVED***
            // todo
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
***REMOVED***
***REMOVED***
