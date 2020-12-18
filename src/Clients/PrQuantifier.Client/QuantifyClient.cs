namespace PrQuantifier.Client
***REMOVED***
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Abstractions;
    using global::PrQuantifier.Core.Git;

    /// <inheritdoc />
    public sealed class QuantifyClient : IQuantifyClient
    ***REMOVED***
        private readonly string gitRepoPath;
        private readonly IPrQuantifier prQuantifier;

        public QuantifyClient(
            IPrQuantifier prQuantifier,
            string gitRepoPath)
        ***REMOVED***
            this.prQuantifier = prQuantifier;
            this.gitRepoPath = gitRepoPath;
            GitEngine = new GitEngine();
***REMOVED***

        /// <inheritdoc />
        public IGitEngine GitEngine ***REMOVED*** get; ***REMOVED***

        /// <inheritdoc />
        public async Task<QuantifierResult> Compute()
        ***REMOVED***
            // get current location changes
            var quantifierInput = GetChanges(gitRepoPath);

            // quantify the changes
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            PrintQuantifierResult(quantifierResult);

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
            return quantifierResult;
***REMOVED***

        /// <inheritdoc />
        public async Task<QuantifierResult> Compute(QuantifierInput quantifierInput)
        ***REMOVED***
            // quantify the changes
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            PrintQuantifierResult(quantifierResult);

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
            return quantifierResult;
***REMOVED***

        private QuantifierInput GetChanges(string repoPath)
        ***REMOVED***
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(GitEngine.GetGitChanges(repoPath));

            return quantifierInput;
***REMOVED***

        private void PrintQuantifierResult(QuantifierResult quantifierResult)
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
***REMOVED***
***REMOVED***
***REMOVED***
