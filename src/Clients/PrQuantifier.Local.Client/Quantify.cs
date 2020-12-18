namespace PrQuantifier.Local.Client
***REMOVED***
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Abstractions;
    using global::PrQuantifier.Core.Git;

    internal sealed class Quantify
    ***REMOVED***
        private readonly string gitRepoPath;
        private readonly IPrQuantifier prQuantifier;

        internal Quantify(
            IPrQuantifier prQuantifier,
            string gitRepoPath)
        ***REMOVED***
            this.prQuantifier = prQuantifier;
            this.gitRepoPath = gitRepoPath;
            GitEngine = new GitEngine();
***REMOVED***

        internal IGitEngine GitEngine ***REMOVED*** get; ***REMOVED***

        internal async Task<string> Compute()
        ***REMOVED***
            // get current location changes
            var quantifierInput = GetChanges(gitRepoPath);

            // quantify the changes
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            var quantifierResultJson = PrintQuantifierResult(quantifierResult);

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
            return quantifierResultJson;
***REMOVED***

        private QuantifierInput GetChanges(string repoPath)
        ***REMOVED***
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(GitEngine.GetGitChanges(repoPath));

            return quantifierInput;
***REMOVED***

        private string PrintQuantifierResult(QuantifierResult quantifierResult)
        ***REMOVED***
            // write the results
            var quantifierResultJson = JsonSerializer.Serialize(quantifierResult);

            Console.ForegroundColor = quantifierResult.Label.Contains("medium", StringComparison.InvariantCultureIgnoreCase)
                ? ConsoleColor.Yellow
                : quantifierResult.Label.Contains("large", StringComparison.InvariantCultureIgnoreCase)
                    ? ConsoleColor.Red
                    : ConsoleColor.Green;

            Console.WriteLine(
                $"Label = ***REMOVED***quantifierResult.Label***REMOVED***\tDiff +***REMOVED***quantifierResult.QuantifiedLinesAdded***REMOVED*** -***REMOVED***quantifierResult.QuantifiedLinesDeleted***REMOVED***" +
                $"\tWithin the team your are at ***REMOVED***quantifierResult.PercentileAddition***REMOVED*** percentile for " +
                $"additions changes and at ***REMOVED***quantifierResult.PercentileDeletion***REMOVED*** for deletions.");
            Console.ResetColor();

            return quantifierResultJson;
***REMOVED***
***REMOVED***
***REMOVED***
