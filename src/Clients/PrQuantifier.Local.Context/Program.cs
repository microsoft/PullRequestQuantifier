namespace PrQuantifier.Local.Context
***REMOVED***
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core;
    using global::PrQuantifier.Core.Abstractions;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Git;

    public static class Program
    ***REMOVED***
        private const string PrQuantifierContexts = "PrQuantifierContexts";
        private static readonly IGitEngine GitEngine = new GitEngine();

        public static async Task Main(string[] args)
        ***REMOVED***
            args = CheckArgs(args);

            // get historical changes
            var historicalChanges = GitEngine.GetGitHistoricalChangesToParent(args[0]);
            var context = InitializeDefaultContext();

            // do the quantification based on th default context to get accurate numbers
            IPrQuantifier prQuantifier = new PrQuantifier(context);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(historicalChanges.Values.SelectMany(v => v));
            await prQuantifier.Quantify(quantifierInput);

            context.AdditionPercentile = new SortedDictionary<int, float>(Percentile(historicalChanges, true));
            context.DeletionPercentile = new SortedDictionary<int, float>(Percentile(historicalChanges, false));

            // serialize the new context
            var filePath = Path.Combine(PrQuantifierContexts, $"Context_***REMOVED***Guid.NewGuid()***REMOVED***.txt");
            context.SerializeToYaml(filePath);
            Console.WriteLine(
                $"Generate context for Repo located on '***REMOVED***GitEngine.GetRepoRoot(args[0])***REMOVED***'" +
                $", context file located at ***REMOVED***Path.Combine(Environment.CurrentDirectory, filePath)***REMOVED***");
***REMOVED***

        private static IDictionary<int, float> Percentile(
            IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> historicalChanges,
            bool addition)
        ***REMOVED***
            var ret = new Dictionary<int, float>();

            var data = historicalChanges
                .SelectMany(h => h.Value)
                .Where(v => (addition ? v.QuantifiedLinesAdded : v.QuantifiedLinesDeleted) > 0)
                .Select(v => addition ? v.QuantifiedLinesAdded : v.QuantifiedLinesDeleted).ToArray();
            Array.Sort(data);

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

        private static Context InitializeDefaultContext()
        ***REMOVED***
            // todo use exiting contexts to augment with the new data.
            var context = new Context
            ***REMOVED***
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
                        Value = 9,
                        GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
            ***REMOVED***,
                    new Threshold
                    ***REMOVED***
                        Label = "Small",
                        Value = 29,
                        GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
            ***REMOVED***,
                    new Threshold
                    ***REMOVED***
                        Label = "Medium",
                        Value = 99,
                        GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
            ***REMOVED***,
                    new Threshold
                    ***REMOVED***
                        Label = "499",
                        Value = 90,
                        GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
            ***REMOVED***,
                    new Threshold
                    ***REMOVED***
                        Label = "Extra Large",
                        Value = 999,
                        GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
            ***REMOVED***
        ***REMOVED***,
                Excluded = new List<string> ***REMOVED*** "*.csproj" ***REMOVED***,
                GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
    ***REMOVED***;

            return context;
***REMOVED***
***REMOVED***
***REMOVED***