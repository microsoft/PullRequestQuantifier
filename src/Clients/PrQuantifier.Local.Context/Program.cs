namespace PrQuantifier.Local.Context
{
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
    {
        private const string PrQuantifierContexts = "PrQuantifierContexts";
        private static readonly IGitEngine GitEngine = new GitEngine();

        public static async Task Main(string[] args)
        {
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
            var filePath = Path.Combine(PrQuantifierContexts, $"Context_{Guid.NewGuid()}.yaml");
            context.SerializeToYaml(filePath);
            Console.WriteLine(
                $"Generate context for Repo located on '{GitEngine.GetRepoRoot(args[0])}'" +
                $", context file located at {Path.Combine(Environment.CurrentDirectory, filePath)}");
        }

        private static IDictionary<int, float> Percentile(
            IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> historicalChanges,
            bool addition)
        {
            var ret = new Dictionary<int, float>();

            var data = historicalChanges
                .SelectMany(h => h.Value)
                .Where(v => (addition ? v.QuantifiedLinesAdded : v.QuantifiedLinesDeleted) > 0)
                .Select(v => addition ? v.QuantifiedLinesAdded : v.QuantifiedLinesDeleted).ToArray();
            Array.Sort(data);

            foreach (var value in data)
            {
                var percentile = ComputePercentile.Percentile(
                    data,
                    value);
                ret[value] = percentile;
            }

            return ret;
        }

        private static string[] CheckArgs(string[] args)
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, PrQuantifierContexts));

            // if repo path is missing then consider the local repo
            if (args == null || args.Length == 0)
            {
                return new[] { Environment.CurrentDirectory };
            }

            if (!Directory.Exists(args[0]))
            {
                throw new DirectoryNotFoundException(args[0]);
            }

            return args;
        }

        private static Context InitializeDefaultContext()
        {
            // todo use exiting contexts to augment with the new data.
            var context = new Context
            {
                LanguageOptions = new LanguageOptions
                {
                    IgnoreCodeBlockSeparator = true,
                    IgnoreComments = true,
                    IgnoreSpaces = true
                },
                DynamicBehaviour = false,
                Thresholds = new List<Threshold>
                {
                    new Threshold
                    {
                        Label = "Extra Small",
                        Value = 9,
                        GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                    },
                    new Threshold
                    {
                        Label = "Small",
                        Value = 29,
                        GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                    },
                    new Threshold
                    {
                        Label = "Medium",
                        Value = 99,
                        GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                    },
                    new Threshold
                    {
                        Label = "499",
                        Value = 90,
                        GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                    },
                    new Threshold
                    {
                        Label = "Extra Large",
                        Value = 999,
                        GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                    }
                },
                Excluded = new List<string> { "*.csproj" },
                GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
            };

            return context;
        }
    }
}