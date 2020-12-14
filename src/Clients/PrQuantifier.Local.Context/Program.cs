namespace PrQuantifier.Local.Context
***REMOVED***
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using PrQuantifier.Core;
    using PrQuantifier.Core.Abstractions;
    using PrQuantifier.Core.Context;
    using PrQuantifier.Core.Git;

    public static class Program
    ***REMOVED***
        private const string PrQuantifierContexts = "PrQuantifierContexts";
        private static readonly IGitEngine GitEngine = new GitEngine();

        public static async Task Main(string[] args)
        ***REMOVED***
            args = CheckArgs(args);

            await Task.Factory.StartNew(() =>
            ***REMOVED***
                var historicalChanges = GitEngine.GetGitHistoricalChangesToParent(args[0]);
                var additionPercentiles = Percentile(historicalChanges, true);
                var deletionPercentiles = Percentile(historicalChanges, false);

                // todo use exiting contexts to augment with the new data.
                var context = new Context
                ***REMOVED***
                    AdditionPercentile = new SortedDictionary<int, float>(additionPercentiles),
                    DeletionPercentile = new SortedDictionary<int, float>(deletionPercentiles),
                    LanguageOptions = new LanguageOptions
                    ***REMOVED***
                        IgnoreCodeBlockSeparator = true,
                        IgnoreComments = true,
                        IgnoreSpaces = true
            ***REMOVED***,
                    DynamicBehaviour = false,
                    Thresholds = new List<Threshold>
                    ***REMOVED***
                        new Threshold
                        ***REMOVED***
                            Label = "Extra Small",
                            Value = 15,
                            GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
                ***REMOVED***,
                        new Threshold
                        ***REMOVED***
                            Label = "Small",
                            Value = 30,
                            GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
                ***REMOVED***,
                        new Threshold
                        ***REMOVED***
                            Label = "Medium",
                            Value = 70,
                            GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
                ***REMOVED***,
                        new Threshold
                        ***REMOVED***
                            Label = "Large",
                            Value = 90,
                            GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
                ***REMOVED***,
                        new Threshold
                        ***REMOVED***
                            Label = "Extra Large",
                            Value = 100,
                            GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
                ***REMOVED***
            ***REMOVED***,
                    Excluded = new List<string> ***REMOVED*** "*.csproj" ***REMOVED***,
                    GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
        ***REMOVED***;
                var filePath = Path.Combine(PrQuantifierContexts, $"Context_***REMOVED***Guid.NewGuid()***REMOVED***.txt");
                context.SerializeToYaml(filePath);
                Console.WriteLine(
                    $"Generate context for Repo located on '***REMOVED***GitEngine.GetRepoRoot(args[0])***REMOVED***'" +
                    $", context file located at ***REMOVED***Path.Combine(Environment.CurrentDirectory, filePath)***REMOVED***");
    ***REMOVED***);
***REMOVED***

        private static IDictionary<int, float> Percentile(
            IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> historicalChanges,
            bool addition)
        ***REMOVED***
            // generate context at moment zero, when there is no history will look at the AbsoluteLinesAdded,
            // AbsoluteLinesDeleted
            var ret = new Dictionary<int, float>();

            var data = historicalChanges
                .SelectMany(h => h.Value)
                .Where(v => (addition ? v.AbsoluteLinesAdded : v.AbsoluteLinesDeleted) > 0)
                .Select(v => addition ? v.AbsoluteLinesAdded : v.AbsoluteLinesDeleted).ToArray();

            foreach (var value in data)
            ***REMOVED***
                var percentile = ComputePercentile.Percentile(
                    data,
                    value);
                ret[value] = percentile;
    ***REMOVED***

            return ret;
***REMOVED***

        private static string[] CheckArgs(string[] args)
        ***REMOVED***
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, PrQuantifierContexts));

            // if repo path is missing then consider the local repo
            if (args == null || args.Length == 0)
            ***REMOVED***
                return new[] ***REMOVED*** Environment.CurrentDirectory ***REMOVED***;
    ***REMOVED***

            if (!Directory.Exists(args[0]))
            ***REMOVED***
                throw new DirectoryNotFoundException(args[0]);
    ***REMOVED***

            return args;
***REMOVED***
***REMOVED***
***REMOVED***