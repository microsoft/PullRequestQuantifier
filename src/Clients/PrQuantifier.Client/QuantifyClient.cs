namespace PrQuantifier.Client
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Abstractions;
    using global::PrQuantifier.Core.Git;

    /// <inheritdoc />
    public sealed class QuantifyClient : IQuantifyClient
    {
        private readonly string gitRepoPath;
        private readonly IPrQuantifier prQuantifier;

        public QuantifyClient(
            IPrQuantifier prQuantifier,
            string gitRepoPath)
        {
            this.prQuantifier = prQuantifier;
            this.gitRepoPath = gitRepoPath;
            GitEngine = new GitEngine();
        }

        /// <inheritdoc />
        public IGitEngine GitEngine { get; }

        /// <inheritdoc />
        public async Task<QuantifierResult> Compute()
        {
            // get current location changes
            var quantifierInput = GetChanges(gitRepoPath);

            // quantify the changes
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            PrintQuantifierResult(quantifierResult);

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
            return quantifierResult;
        }

        /// <inheritdoc />
        public async Task<QuantifierResult> Compute(QuantifierInput quantifierInput)
        {
            // quantify the changes
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            PrintQuantifierResult(quantifierResult);

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
            return quantifierResult;
        }

        private QuantifierInput GetChanges(string repoPath)
        {
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(GitEngine.GetGitChanges(repoPath));

            return quantifierInput;
        }

        private void PrintQuantifierResult(QuantifierResult quantifierResult)
        {
            // write the results
            var quantifierResultJson = JsonSerializer.Serialize(quantifierResult);

            Console.ForegroundColor = quantifierResult.Label.Contains("medium", StringComparison.InvariantCultureIgnoreCase)
                ? ConsoleColor.Yellow
                : quantifierResult.Label.Contains("large", StringComparison.InvariantCultureIgnoreCase)
                    ? ConsoleColor.Red
                    : ConsoleColor.Green;

            Console.WriteLine(
                $"Label = {quantifierResult.Label}\tDiff +{quantifierResult.QuantifiedLinesAdded} -{quantifierResult.QuantifiedLinesDeleted}" +
                $"\tWithin the team your are at {quantifierResult.PercentileAddition} percentile for " +
                $"additions changes and at {quantifierResult.PercentileDeletion} for deletions.");
            Console.ResetColor();
        }
    }
}
