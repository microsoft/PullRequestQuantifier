namespace PullRequestQuantifier.Local.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using LibGit2Sharp;
    using PullRequestQuantifier.Abstractions.Core;
    using PullRequestQuantifier.Abstractions.Git;
    using PullRequestQuantifier.Client.Helpers;
    using PullRequestQuantifier.Client.QuantifyClient;

    /// <summary>
    /// Parameters accepted: GitRepoPath={} ContextPath={}
    /// Service=True/False(default is false)
    /// output=summaryByExt/summaryByFile/detailed (default is detailed).
    /// </summary>
    public static class Program
    {
        private static DateTimeOffset changedEventDateTime = DateTimeOffset.MaxValue;

        private static IQuantifyClient quantifyClient;

        private static string contextFilePath;

        public static async Task Main(string[] args)
        {
            var commandLine = new CommandLine(args);

            contextFilePath = commandLine.ContextPath;

            if (commandLine.QuantifierInputFile != null)
            {
                // quantifier input is specified as a file
                // run once and exit
                var quantifierInput = new QuantifierInput();
                var changes = JsonSerializer.Deserialize<List<GitFilePatch>>(
                    await File.ReadAllTextAsync(commandLine.QuantifierInputFile));
                changes?.ForEach(c => quantifierInput.Changes.Add(c));

                quantifyClient = new QuantifyClient(contextFilePath);
                PrintResult(
                    await quantifyClient.Compute(quantifierInput),
                    commandLine.Output);
            }
            else
            {
                var gitRepoPath = commandLine.GitRepoPath ?? Environment.CurrentDirectory;
                var repoRootPath = Repository.Discover(gitRepoPath);

                // if no repo was found  to this path then exit, don't crash
                if (string.IsNullOrWhiteSpace(repoRootPath))
                {
                    Console.WriteLine("GitRepoPath couldn't be found!");
                    return;
                }

                contextFilePath ??= Path.Combine(
                    new DirectoryInfo(repoRootPath).Parent?.FullName,
                    ".prquantifier");

                // run this as a service in case is configured otherwise only run once
                if (commandLine.Service)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Factory.StartNew(() => QuantifyLoop(repoRootPath, commandLine.Output));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                    await Run(repoRootPath);
                }

                // in case service is not set or false then run once and exit
                quantifyClient = new QuantifyClient(contextFilePath);
                PrintResult(
                    await quantifyClient.Compute(repoRootPath),
                    commandLine.Output);
            }
        }

        private static void QuantifyLoop(
            string gitRepoPath,
            ClientOutputType clientOutputType)
        {
            while (true)
            {
                if ((DateTimeOffset.Now - changedEventDateTime).TotalSeconds < 1)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                    continue;
                }

                quantifyClient = new QuantifyClient(contextFilePath);
                PrintResult(
                    quantifyClient.Compute(gitRepoPath).Result,
                    clientOutputType);
                changedEventDateTime = DateTimeOffset.MaxValue;
            }
        }

        private static async Task Run(string gitRepoPath)
        {
            await Task.Factory.StartNew(() =>
            {
                // Create a new FileSystemWatcher and set its properties.
                using FileSystemWatcher watcher = new FileSystemWatcher
                {
                    Path = new DirectoryInfo(Repository.Discover(gitRepoPath)).Parent
                        .FullName,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                    IncludeSubdirectories = true,
                    Filter = "*.*"
                };

                // Add event handlers.
                watcher.Changed += OnChanged;
                watcher.Renamed += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Created += OnChanged;

                // Begin watching.
                watcher.EnableRaisingEvents = true;

                // Wait for the user to quit the program.
                Console.WriteLine(
                    "Press 'q' to quit, 'c' to see the team context, 't' to see the configured thresholds.");
                int key;
                while ((key = Console.Read()) != 'q')
                {
                    switch (key)
                    {
                        case 'c':
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine();
                            Console.WriteLine("Quantifier Context:");
                            Console.WriteLine(File.ReadAllText(contextFilePath));
                            Console.ResetColor();
                            break;
                        }

                        case 'p':
                        {
                            PrintPercentiles(true);
                            PrintPercentiles(false);
                            break;
                        }

                        case 't':
                        {
                            PrintThresholds();
                            break;
                        }
                    }
                }
            });
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            changedEventDateTime = DateTimeOffset.Now;
        }

        private static void PrintThresholds()
        {
            var thresholds = quantifyClient.Context.Thresholds;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine("Thresholds: " + string.Join(" ", thresholds.Select(t => $"\"{t.Label} = {t.Value}\"")));
            Console.ResetColor();
        }

        private static void PrintPercentiles(bool addition)
        {
            var percentile = addition
                ? quantifyClient.Context.AdditionPercentile
                : quantifyClient.Context.DeletionPercentile;
            string label = addition ? "Addition" : "Deletion";

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine($"{label} Percentile:");
            Console.WriteLine(string.Join(" ", percentile.Select(t => $"{t.Key} = {t.Value}\n")));
            Console.ResetColor();
        }

        private static void PrintResult(
            QuantifierResult quantifierResult,
            ClientOutputType clientOutputType)
        {
            Console.ForegroundColor = QuantifyClientHelper.GetColor(quantifierResult.Color);

            switch (clientOutputType)
            {
                case ClientOutputType.None:
                {
                    Console.WriteLine(
                        $"PrQuantified = {quantifierResult.Label},\t" +
                        $"Diff +{quantifierResult.QuantifiedLinesAdded} -{quantifierResult.QuantifiedLinesDeleted} (Formula = {quantifierResult.Formula})," +
                        $"\tTeam percentiles: additions = {quantifierResult.PercentileAddition}%" +
                        $", deletions = {quantifierResult.PercentileDeletion}%," +
                        $" formula = {quantifierResult.FormulaPercentile}%.{Environment.NewLine}");
                    break;
                }

                case ClientOutputType.Detailed:
                {
                    Console.WriteLine(JsonSerializer.Serialize(
                        quantifierResult,
                        new JsonSerializerOptions { WriteIndented = true }));
                    break;
                }

                case ClientOutputType.SummaryByFile:
                {
                    Console.WriteLine(JsonSerializer.Serialize(
                        new
                        {
                            quantifierResult.Label,
                            quantifierResult.QuantifiedLinesAdded,
                            quantifierResult.QuantifiedLinesDeleted,
                            Formula = quantifierResult.Formula.ToString(),
                            quantifierResult.PercentileAddition,
                            quantifierResult.PercentileDeletion,
                            Details = quantifierResult.QuantifierInput.Changes.Select(s =>
                                new
                                {
                                    s.FilePath,
                                    s.QuantifiedLinesAdded,
                                    s.QuantifiedLinesDeleted,
                                    s.DiscardFromCounting
                                })
                        },
                        new JsonSerializerOptions { WriteIndented = true }));
                    break;
                }

                case ClientOutputType.SummaryByExt:
                {
                    Console.WriteLine(JsonSerializer.Serialize(
                        new
                        {
                            quantifierResult.Label,
                            quantifierResult.QuantifiedLinesAdded,
                            quantifierResult.QuantifiedLinesDeleted,
                            Formula = quantifierResult.Formula.ToString(),
                            quantifierResult.PercentileAddition,
                            quantifierResult.PercentileDeletion,
                            Details = quantifierResult.QuantifierInput.Changes.GroupBy(c => c.FileExtension).Select(g =>
                                new
                                {
                                    FileExtension = g.Key,
                                    QuantifiedLinesAdded = g.Sum(v => v.QuantifiedLinesAdded),
                                    QuantifiedLinesDeleted = g.Sum(v => v.QuantifiedLinesDeleted)
                                })
                        },
                        new JsonSerializerOptions { WriteIndented = true }));
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(clientOutputType), clientOutputType, null);
            }

            Console.ResetColor();
        }
    }
}