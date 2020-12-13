namespace PrQuantifier.Local.Client
***REMOVED***
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Git;

    public static class Program
    ***REMOVED***
        private const string PrQuantifierResults = "PrQuantifierResults";

        public static async Task Main(string[] args)
        ***REMOVED***
            CheckArgs(args);

            // get current location changes
            var quantifierInput = GetChanges(Environment.CurrentDirectory);

            // quantify the changes
            var prQuantifier = new PrQuantifier(ContextFactory.Load(args[0]));
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // write the results
            var quantifierResultJson = JsonSerializer.Serialize(quantifierResult);

            File.WriteAllText(
                Path.Combine(PrQuantifierResults, $"***REMOVED***Guid.NewGuid()***REMOVED***.prQuantifier.json"),
                quantifierResultJson);

            Console.WriteLine(quantifierResultJson);

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
***REMOVED***

        private static QuantifierInput GetChanges(string repoPath)
        ***REMOVED***
            var gitEngine = new GitEngine();
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(repoPath));

            return quantifierInput;
***REMOVED***

        private static void CheckArgs(string[] args)
        ***REMOVED***
            if (args == null || args.Length == 0)
            ***REMOVED***
                throw new ArgumentException("Context model file path is missing!");
    ***REMOVED***

            if (!File.Exists(args[0]))
            ***REMOVED***
                throw new FileNotFoundException(args[0]);
    ***REMOVED***

            Directory.CreateDirectory(PrQuantifierResults);
***REMOVED***
***REMOVED***
***REMOVED***
