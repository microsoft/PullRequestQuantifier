namespace PrQuantifier.Local.Context
***REMOVED***
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using PrQuantifier.Core.Git;

    public static class Program
    ***REMOVED***
        private const string PrQuantifierContexts = "PrQuantifierContexts";

        public static async Task Main(string[] args)
        ***REMOVED***
            args = CheckArgs(args);

            await Task.Factory.StartNew(() =>
            ***REMOVED***
                var historicalChanges = GetHistoricalChanges(args[0]);
    ***REMOVED***);
***REMOVED***

        private static IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> GetHistoricalChanges(string repoPath)
        ***REMOVED***
            var gitEngine = new GitEngine();
            return gitEngine.GetGitHistoricalChangesToParent(repoPath);
***REMOVED***

        private static string[] CheckArgs(string[] args)
        ***REMOVED***
            // if repo path is missing then consider the local repo
            if (args == null || args.Length == 0)
            ***REMOVED***
                return new[] ***REMOVED*** Environment.CurrentDirectory ***REMOVED***;
    ***REMOVED***

            if (!Directory.Exists(args[0]))
            ***REMOVED***
                throw new DirectoryNotFoundException(args[0]);
    ***REMOVED***

            Directory.CreateDirectory(PrQuantifierContexts);

            return args;
***REMOVED***
***REMOVED***
***REMOVED***