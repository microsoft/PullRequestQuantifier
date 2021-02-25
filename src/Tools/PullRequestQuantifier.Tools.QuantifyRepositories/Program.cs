namespace PullRequestQuantifier.Tools.QuantifyRepositories
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using LibGit2Sharp;
    using PullRequestQuantifier.Abstractions.Core;
    using PullRequestQuantifier.Client.QuantifyClient;
    using PullRequestQuantifier.GitEngine.Extensions;
    using PullRequestQuantifier.Tools.Common;
    using PullRequestQuantifier.Tools.Common.Model;
    using YamlDotNet.Serialization;

    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var commandLine = new CommandLine(args);

            var organizations = new DeserializerBuilder()
                .Build()
                .Deserialize<List<Organization>>(await File.ReadAllTextAsync(commandLine.ConfigFile));

            await CloneAdoRepo.Program.Main(args);
            await Quantify(
                organizations,
                commandLine.ClonePath);
        }

        private static async Task Quantify(
            IEnumerable<Organization> organizations,
            string clonePath)
        {
            var repositories = organizations.Select(
                    o => o.Projects.Select(
                        p => p.Repositories.Select(
                            r => new
                            {
                                Organization = o.Name,
                                Project = p.Name,
                                Repository = r.Name
                            })))
                .SelectMany(a => a)
                .SelectMany(a => a);

            var quantifyClient = new QuantifyClient(string.Empty);

            foreach (var repository in repositories)
            {
                var resultFile = Path.Combine(clonePath, $"{repository.Repository}_QuantifierResults.csv");
                await InitializeResultFile(resultFile);

                var repoPath = Path.Combine(clonePath, repository.Repository);
                var repoRoot = LibGit2Sharp.Repository.Discover(repoPath);
                if (repoRoot == null)
                {
                    Console.WriteLine($"No repo found at {repoPath}");
                    continue;
                }

                using var repo = new LibGit2Sharp.Repository(repoRoot);
                var commits = repo.Commits.QueryBy(
                    new CommitFilter
                    {
                        FirstParentOnly = true
                    });

                Console.WriteLine($"Total commits to evaluate : {commits.Count()}. Repo name {repository}.");
                var sw = new Stopwatch();
                sw.Reset();
                sw.Start();
                var batchSize = 100;
                var quantifierResults = new ConcurrentDictionary<string, QuantifierResult>();
                for (int page = 0; page < (commits.Count() / batchSize) + 1; page++)
                {
                    var commitBatch = commits.Skip(batchSize * page).Take(batchSize);
                    var quantifyTasks = commitBatch.Select(
                        async commit =>
                        {
                            try
                            {
                                var quantifierInput = new QuantifierInput();
                                foreach (var parent in commit.Parents)
                                {
                                    var patch = repo.Diff.Compare<Patch>(parent.Tree, commit.Tree);

                                    foreach (var gitFilePatch in patch.GetGitFilePatch())
                                    {
                                        quantifierInput.Changes.Add(gitFilePatch);
                                    }
                                }

                                var quantifierResult = await quantifyClient.Compute(quantifierInput);
                                quantifierResults.TryAdd(commit.Sha, quantifierResult);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                    await Task.WhenAll(quantifyTasks);
                    await AddResultsToFile(quantifierResults, resultFile);
                    quantifierResults = new ConcurrentDictionary<string, QuantifierResult>();
                    Console.WriteLine($"{(page + 1) * batchSize}/{commits.Count()} {sw.Elapsed}");
                }
            }
        }

        private static async Task InitializeResultFile(string csvResultPath)
        {
            await using var streamWriter = new StreamWriter(csvResultPath, true);
            await streamWriter.WriteLineAsync(
                "CommitSha1,QuantifiedLinesAdded,QuantifiedLinesDeleted,PercentileAddition," +
                "PercentileDeletion,DiffPercentile,Label,AbsoluteLinesAdded,AbsoluteLinesDeleted");
        }

        private static async Task AddResultsToFile(IReadOnlyDictionary<string, QuantifierResult> results, string csvResultPath)
        {
            await using var streamWriter = new StreamWriter(csvResultPath, true);
            foreach (var result in results)
            {
                await streamWriter.WriteLineAsync(
                    $"{result.Key}," +
                    $"{result.Value.QuantifiedLinesAdded}," +
                    $"{result.Value.QuantifiedLinesDeleted}," +
                    $"{Math.Round(result.Value.PercentileAddition, 2)}," +
                    $"{Math.Round(result.Value.PercentileDeletion, 2)}," +
                    $"{Math.Round(result.Value.FormulaPercentile, 2)}," +
                    $"{result.Value.Label}," +
                    $"{result.Value.QuantifierInput.Changes.Sum(c => c.AbsoluteLinesAdded)}," +
                    $"{result.Value.QuantifierInput.Changes.Sum(c => c.AbsoluteLinesDeleted)},");
            }
        }
    }
}
