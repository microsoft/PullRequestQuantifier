namespace PullRequestQuantifier.Abstractions.Git
***REMOVED***
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ignore;

    // todo for some of the extensions we need language specific implementation for now just do it for c sharp
    // todo optimize this part to have only one iteration pass over the changes and lines, trim once ...
    public static class GitFilePatchExtensions
    ***REMOVED***
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
        ***REMOVED***
            if (gitFilePatch == null)
            ***REMOVED***
                throw new ArgumentNullException(nameof(gitFilePatch));
    ***REMOVED***

            var ignore = new Ignore();

            // if included patterns are specified, do not consider excluded patterns
            var includedPatternList = includedPatterns?.ToList();
            if (includedPatternList != null && includedPatternList.Any())
            ***REMOVED***
                ignore.Add(includedPatternList);

                // not ignored by ignore effectively means this is not a match
                // and must be excluded
                if (!ignore.IsIgnored(gitFilePatch.FilePath))
                ***REMOVED***
                    gitFilePatch.DiscardFromCounting = true;
                    return true;
        ***REMOVED***

                return false;
    ***REMOVED***

            var excludedPatternList = excludedPatterns?.ToList();
            if (excludedPatternList == null)
            ***REMOVED***
                return false;
    ***REMOVED***

            ignore.Add(excludedPatternList);
            if (ignore.IsIgnored(gitFilePatch.FilePath))
            ***REMOVED***
                gitFilePatch.DiscardFromCounting = true;
                return true;
    ***REMOVED***

            return false;
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

            gitFilePatch.DiscardFromCounting = true;
***REMOVED***

        public static void RemoveCopiedChanges(this GitFilePatch gitFilePatch)
        ***REMOVED***
            if (gitFilePatch.ChangeType != GitChangeType.Copied)
            ***REMOVED***
                return;
    ***REMOVED***

            gitFilePatch.DiscardFromCounting = true;
***REMOVED***

        public static void ComputeChanges(this GitFilePatch gitFilePatch)
        ***REMOVED***
            if (gitFilePatch == null)
            ***REMOVED***
                throw new ArgumentNullException(nameof(gitFilePatch));
    ***REMOVED***

            // if file change was discarded from the counted do nothing.
            if (gitFilePatch.DiscardFromCounting)
            ***REMOVED***
                Console.WriteLine();
                return;
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
