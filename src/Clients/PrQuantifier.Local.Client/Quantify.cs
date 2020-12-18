namespace PrQuantifier.Local.Client
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Abstractions;
    using global::PrQuantifier.Core.Git;

    internal sealed class Quantify
    {
        private readonly string gitRepoPath;
        private readonly IPrQuantifier prQuantifier;

        internal Quantify(
            IPrQuantifier prQuantifier,
            string gitRepoPath)
        {
            this.prQuantifier = prQuantifier;
            this.gitRepoPath = gitRepoPath;
            GitEngine = new GitEngine();
        }

        internal IGitEngine GitEngine { get; }

        internal async Task<string> Compute()
        {
            // get current location changes
            var quantifierInput = GetChanges(gitRepoPath);

            // quantify the changes
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            var quantifierResultJson = PrintQuantifierResult(quantifierResult);

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
            return quantifierResultJson;
        }

        private QuantifierInput GetChanges(string repoPath)
        {
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(GitEngine.GetGitChanges(repoPath));

            return quantifierInput;
        }

        private string PrintQuantifierResult(QuantifierResult quantifierResult)
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

            return quantifierResultJson;
        }
    }
}
