namespace PrQuantifier.Local.Client
***REMOVED***
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::PrQuantifier.Client;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Git;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Parameters accepted: GitRepoPath=***REMOVED******REMOVED*** ContextPath=***REMOVED******REMOVED*** Service=True/False(default is false) PrintJson=True/False(default is false).
    /// </summary>
    public static class Program
    ***REMOVED***
        private static ServiceProvider serviceProvider;
        private static DateTimeOffset changedEventDateTime = DateTimeOffset.MaxValue;

        public static async Task Main(string[] args)
        ***REMOVED***
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            var config = builder.Build();

            var gitRepoPath = config["GitRepoPath"] ?? Environment.CurrentDirectory;

            var repoRootPath = new GitEngine().GetRepoRoot(gitRepoPath);

            // if no repo was found  to this path then exit, don't crash
            if (string.IsNullOrWhiteSpace(repoRootPath))
            ***REMOVED***
                Console.WriteLine("GitRepoPath couldn't be found!");
                return;
    ***REMOVED***

            var contextPath = config["ContextPath"]
                              ?? Path.Combine(
                                  new DirectoryInfo(repoRootPath).Parent?.FullName,
                                  ".prquantifier");
            Dependencies(
                contextPath,
                gitRepoPath,
                config["PrintJson"] ?? "false");

            // run this as a service in case is configured otherwise only run once
            var service = config["Service"];
            if (!string.IsNullOrWhiteSpace(service) &&
                service.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            ***REMOVED***
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Factory.StartNew(Quantify);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                await Run(gitRepoPath, contextPath);
    ***REMOVED***

            // in case service is not set or false then run once and exit
            await serviceProvider.GetService<IQuantifyClient>().Compute();
***REMOVED***

        private static void Quantify()
        ***REMOVED***
            while (true)
            ***REMOVED***
                if ((DateTimeOffset.Now - changedEventDateTime).TotalSeconds < 1)
                ***REMOVED***
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                    continue;
        ***REMOVED***

                serviceProvider.GetService<IQuantifyClient>()?.Compute().Wait();
                changedEventDateTime = DateTimeOffset.MaxValue;
    ***REMOVED***
***REMOVED***

        private static void Dependencies(
            string contextFilePath,
            string gitRepoPath,
            string printJson)
        ***REMOVED***
            bool.TryParse(printJson, out var jsonFormat);

            serviceProvider = new ServiceCollection()
                .AddSingleton<IPrQuantifier>(p => new PrQuantifier(ContextFactory.Load(contextFilePath)))
                .AddSingleton<IQuantifyClient>(p => new QuantifyClient(
                    p.GetRequiredService<IPrQuantifier>(),
                    gitRepoPath,
                    jsonFormat))
                .BuildServiceProvider();
***REMOVED***

        private static async Task Run(
            string gitRepoPath,
            string contextFilePath)
        ***REMOVED***
            await Task.Factory.StartNew(() =>
            ***REMOVED***
                // Create a new FileSystemWatcher and set its properties.
                using FileSystemWatcher watcher = new FileSystemWatcher
                ***REMOVED***
                    Path = new DirectoryInfo(serviceProvider.GetService<IQuantifyClient>()?.GitEngine.GetRepoRoot(gitRepoPath)).Parent
                        .FullName,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                    IncludeSubdirectories = true,
                    Filter = "*.*"
        ***REMOVED***;

                // Add event handlers.
                watcher.Changed += OnChanged;
                watcher.Renamed += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Created += OnChanged;

                // Begin watching.
                watcher.EnableRaisingEvents = true;

                // Wait for the user to quit the program.
                Console.WriteLine("Press 'q' to quit, 'c' to see the team context, 't' to see the configured thresholds.");
                int key;
                while ((key = Console.Read()) != 'q')
                ***REMOVED***
                    switch (key)
                    ***REMOVED***
                        case 'c':
                        ***REMOVED***
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine();
                            Console.WriteLine("Quantifier Context:");
                            Console.WriteLine(File.ReadAllText(contextFilePath));
                            Console.ResetColor();
                            break;
                ***REMOVED***

                        case 'p':
                        ***REMOVED***
                            PrintPercentiles(true);
                            PrintPercentiles(false);
                            break;
                ***REMOVED***

                        case 't':
                        ***REMOVED***
                            PrintThresholds();
                            break;
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***
    ***REMOVED***);
***REMOVED***

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        ***REMOVED***
            changedEventDateTime = DateTimeOffset.Now;
***REMOVED***

        private static void PrintThresholds()
        ***REMOVED***
            var thresholds = serviceProvider?.GetService<IPrQuantifier>().Context.Thresholds;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine("Thresholds: " + string.Join(" ", thresholds.Select(t => $"\"***REMOVED***t.Label***REMOVED*** = ***REMOVED***t.Value***REMOVED***\"")));
            Console.ResetColor();
***REMOVED***

        private static void PrintPercentiles(bool addition)
        ***REMOVED***
            var prQuantifier = serviceProvider?.GetService<IPrQuantifier>();
            var percentile = addition ? prQuantifier.Context.AdditionPercentile : prQuantifier.Context.DeletionPercentile;
            string label = addition ? "Addition" : "Deletion";

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine($"***REMOVED***label***REMOVED*** Percentile:");
            Console.WriteLine(string.Join(" ", percentile.Select(t => $"***REMOVED***t.Key***REMOVED*** = ***REMOVED***t.Value***REMOVED***\n")));
            Console.ResetColor();
***REMOVED***
***REMOVED***
***REMOVED***
