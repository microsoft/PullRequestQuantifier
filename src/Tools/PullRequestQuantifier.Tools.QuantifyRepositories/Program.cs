namespace PullRequestQuantifier.Tools.QuantifyRepositories
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using PullRequestQuantifier.Abstractions.Core;
    using PullRequestQuantifier.Client.QuantifyClient;
    using PullRequestQuantifier.GitEngine;
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
            var repositories = organizations.Select(o =>
                o.Projects.Select(p =>
                p.Repositories.Select(r => new
                {
                    Organization = o.Name,
                    Project = p.Name,
                    Repository = r.Name
                })))
                .SelectMany(a => a)
                .SelectMany(a => a);

            var gitEngine = new GitEngine();
            var quantifyClient = new QuantifyClient(string.Empty);

            foreach (var repository in repositories)
            {
                var saveResultsToDatabase = new Dictionary<string, QuantifierResult>();

                var repoPath = Path.Combine(clonePath, repository.Repository);
                var commitsSha1 = gitEngine.GetAllCommits(repoPath).Select(c => c.Sha).ToArray();
                Console.WriteLine($"Total commits to evaluate : {commitsSha1.Length}. Repo name {repository.Repository}.");

                foreach (var commitSha1 in commitsSha1)
                {
                    try
                    {
                        var quantifierResult = await quantifyClient.Compute(repoPath, commitSha1);
                        saveResultsToDatabase[commitSha1] = quantifierResult;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                await SaveToCsv(
                    saveResultsToDatabase,
                    Path.Combine(clonePath, $"{repository.Repository}_QuantifierResults.csv"));
            }
        }

        private static async Task SaveToCsv(
            IReadOnlyDictionary<string, QuantifierResult> results,
            string csvResultPath)
        {
            await using var streamWriter = new StreamWriter(csvResultPath);
            await streamWriter.WriteLineAsync(
                "CommitSha1,QuantifiedLinesAdded,QuantifiedLinesDeleted,PercentileAddition," +
                "PercentileDeletion,DiffPercentile,Label,AbsoluteLinesAdded,AbsoluteLinesDeleted");

            foreach (var result in results)
            {
                await streamWriter.WriteLineAsync($"{result.Key}," +
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
