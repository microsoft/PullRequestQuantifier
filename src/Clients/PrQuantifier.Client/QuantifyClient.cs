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
        private readonly bool printJson;

        public QuantifyClient(
            IPrQuantifier prQuantifier,
            string gitRepoPath,
            bool printJson)
        {
            this.prQuantifier = prQuantifier;
            this.gitRepoPath = gitRepoPath;
            this.printJson = printJson;
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
            if (printJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(quantifierResult, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.ForegroundColor = GetColor(quantifierResult.Color);
                Console.WriteLine(
                    $"PrQuantified = {quantifierResult.Label}\t" +
                    $"Diff +{quantifierResult.QuantifiedLinesAdded} -{quantifierResult.QuantifiedLinesDeleted} (Formula = {quantifierResult.Formula})" +
                    $"\tTeam percentiles: additions = {quantifierResult.PercentileAddition}%" +
                    $", deletions = {quantifierResult.PercentileDeletion}%.");
                Console.ResetColor();
            }
        }

        private ConsoleColor GetColor(string color)
        {
            return color switch
            {
                nameof(ConsoleColor.Black) => ConsoleColor.Black,
                nameof(ConsoleColor.DarkBlue) => ConsoleColor.DarkBlue,
                nameof(ConsoleColor.DarkGreen) => ConsoleColor.DarkGreen,
                nameof(ConsoleColor.DarkCyan) => ConsoleColor.DarkCyan,
                nameof(ConsoleColor.DarkRed) => ConsoleColor.DarkRed,
                nameof(ConsoleColor.DarkMagenta) => ConsoleColor.DarkMagenta,
                nameof(ConsoleColor.DarkYellow) => ConsoleColor.DarkYellow,
                nameof(ConsoleColor.Gray) => ConsoleColor.Gray,
                nameof(ConsoleColor.DarkGray) => ConsoleColor.DarkGray,
                nameof(ConsoleColor.Blue) => ConsoleColor.Blue,
                nameof(ConsoleColor.Green) => ConsoleColor.Green,
                nameof(ConsoleColor.Cyan) => ConsoleColor.Cyan,
                nameof(ConsoleColor.Red) => ConsoleColor.Red,
                nameof(ConsoleColor.Magenta) => ConsoleColor.Magenta,
                nameof(ConsoleColor.Yellow) => ConsoleColor.Yellow,
                nameof(ConsoleColor.White) => ConsoleColor.White,
                _ => ConsoleColor.DarkGray,
            };
        }
    }
}
