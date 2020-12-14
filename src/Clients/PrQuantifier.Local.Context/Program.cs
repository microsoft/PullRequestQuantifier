namespace PrQuantifier.Local.Context
{
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
    {
        private const string PrQuantifierContexts = "PrQuantifierContexts";
        private static readonly IGitEngine GitEngine = new GitEngine();

        public static async Task Main(string[] args)
        {
            args = CheckArgs(args);

            await Task.Factory.StartNew(() =>
            {
                var historicalChanges = GitEngine.GetGitHistoricalChangesToParent(args[0]);
                var additionPercentiles = Percentile(historicalChanges, true);
                var deletionPercentiles = Percentile(historicalChanges, false);

                // todo use exiting contexts to augment with the new data.
                var context = new Context
                {
                    AdditionPercentile = new SortedDictionary<int, float>(additionPercentiles),
                    DeletionPercentile = new SortedDictionary<int, float>(deletionPercentiles),
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
                            Value = 15,
                            GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                        },
                        new Threshold
                        {
                            Label = "Small",
                            Value = 30,
                            GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                        },
                        new Threshold
                        {
                            Label = "Medium",
                            Value = 70,
                            GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                        },
                        new Threshold
                        {
                            Label = "Large",
                            Value = 90,
                            GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                        },
                        new Threshold
                        {
                            Label = "Extra Large",
                            Value = 100,
                            GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                        }
                    },
                    Excluded = new List<string> { "*.csproj" },
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                };
                var filePath = Path.Combine(PrQuantifierContexts, $"Context_{Guid.NewGuid()}.txt");
                context.SerializeToYaml(filePath);
                Console.WriteLine(
                    $"Generate context for Repo located on '{GitEngine.GetRepoRoot(args[0])}'" +
                    $", context file located at {Path.Combine(Environment.CurrentDirectory, filePath)}");
            });
        }

        private static IDictionary<int, float> Percentile(
            IReadOnlyDictionary<GitCommit, IEnumerable<GitFilePatch>> historicalChanges,
            bool addition)
        {
            // generate context at moment zero, when there is no history will look at the AbsoluteLinesAdded,
            // AbsoluteLinesDeleted
            var ret = new Dictionary<int, float>();

            var data = historicalChanges
                .SelectMany(h => h.Value)
                .Where(v => (addition ? v.AbsoluteLinesAdded : v.AbsoluteLinesDeleted) > 0)
                .Select(v => addition ? v.AbsoluteLinesAdded : v.AbsoluteLinesDeleted).ToArray();

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
    }
}