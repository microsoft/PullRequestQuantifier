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
    using PullRequestQuantifier.Client.QuantifyClient;

    /// <summary>
    /// Parameters accepted: GitRepoPath={} ContextPath={}
    /// Service=True/False(default is false) PrintJson=True/False(default is false)
    /// output=summaryByExt/summaryByFile/detailed (default is detailed).
    /// </summary>
    public static class Program
    {
        private static DateTimeOffset changedEventDateTime = DateTimeOffset.MaxValue;

        private static IQuantifyClient quantifyClient;

        public static async Task Main(string[] args)
        {
            var commandLine = new CommandLine(args);

            var contextPath = commandLine.ContextPath;

            if (commandLine.QuantifierInputFile != null)
            {
                // quantifier input is specified as a file
                // run once and exit
                var quantifierInput = new QuantifierInput();
                var changes = JsonSerializer.Deserialize<List<GitFilePatch>>(
                    await File.ReadAllTextAsync(commandLine.QuantifierInputFile));
                changes?.ForEach(c => quantifierInput.Changes.Add(c));

                quantifyClient = new QuantifyClient(
                    contextPath,
                    commandLine.PrintJson,
                    commandLine.Output);
                await quantifyClient.Compute(quantifierInput);
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

                contextPath ??= Path.Combine(
                    new DirectoryInfo(repoRootPath).Parent?.FullName,
                    ".prquantifier");

                quantifyClient = new QuantifyClient(
                    contextPath,
                    commandLine.PrintJson,
                    commandLine.Output);

                // run this as a service in case is configured otherwise only run once
                if (commandLine.Service)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Factory.StartNew(() => QuantifyLoop(repoRootPath));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                    await Run(repoRootPath, contextPath);
                }

                // in case service is not set or false then run once and exit
                await quantifyClient.Compute(repoRootPath);
            }
        }

        private static void QuantifyLoop(string gitRepoPath)
        {
            while (true)
            {
                if ((DateTimeOffset.Now - changedEventDateTime).TotalSeconds < 1)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                    continue;
                }

                quantifyClient.Compute(gitRepoPath).Wait();
                changedEventDateTime = DateTimeOffset.MaxValue;
            }
        }

        private static async Task Run(
            string gitRepoPath,
            string contextFilePath)
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
    }
}