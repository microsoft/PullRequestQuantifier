namespace PrQuantifier.Local.Context
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using PrQuantifier.Core.Git;

    public static class Program
    {
        private const string PrQuantifierContexts = "PrQuantifierContexts";

        public static async Task Main(string[] args)
        {
            args = CheckArgs(args);

            await Task.Factory.StartNew(() =>
            {
                var historicalChanges = GetHistoricalChanges(args[0]);
            });
        }

        private static IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> GetHistoricalChanges(string repoPath)
        {
            var gitEngine = new GitEngine();
            return gitEngine.GetGitHistoricalChangesToParent(repoPath);
        }

        private static string[] CheckArgs(string[] args)
        {
            // if repo path is missing then consider the local repo
            if (args == null || args.Length == 0)
            {
                return new[] { Environment.CurrentDirectory };
            }

            if (!Directory.Exists(args[0]))
            {
                throw new DirectoryNotFoundException(args[0]);
            }

            Directory.CreateDirectory(PrQuantifierContexts);

            return args;
        }
    }
}