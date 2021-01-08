namespace PullRequestQuantifier.Client
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;
    using global::PullRequestQuantifier.Abstractions.Exceptions;
    using global::PullRequestQuantifier.GitEngine;
    using YamlDotNet.Core;

    /// <inheritdoc />
    public sealed class QuantifyClient : IQuantifyClient
    {
        private readonly IPullRequestQuantifier prQuantifier;
        private readonly bool printJson;
        private readonly GitEngine gitEngine;

        public QuantifyClient(
            string contextFilePath,
            bool printJson)
        {
            var context = LoadContext(contextFilePath);
            prQuantifier = new PullRequestQuantifier(context);
            this.printJson = printJson;
            gitEngine = new GitEngine();
        }

        /// <inheritdoc />
        public Context Context => prQuantifier.Context;

        /// <inheritdoc />
        public async Task<QuantifierResult> Compute(string gitRepoPath)
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

        private Context LoadContext(string contextFilePathOrContent)
        {
            var context = DefaultContext.Value;
            if (string.IsNullOrEmpty(contextFilePathOrContent))
            {
                return context;
            }

            // if no valid context will be loaded then load the default one, don't fail
            try
            {
                var ctx = ContextFactory.Load(contextFilePathOrContent);
                context = ctx;
            }
            catch (YamlException)
            {
            }
            catch (ThresholdException ex)
            {
                // misconfiguration then print the exception
                Console.WriteLine(ex);
            }
            catch (ArgumentNullException ex)
            {
                // misconfiguration then print the exception
                Console.WriteLine(ex);
            }

            return context;
        }

        private QuantifierInput GetChanges(string repoPath)
        {
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(repoPath));

            return quantifierInput;
        }

        private void PrintQuantifierResult(QuantifierResult quantifierResult)
        {
            if (printJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(
                    quantifierResult,
                    new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.ForegroundColor = GetColor(quantifierResult.Color);

                var details = string.Join(
                    Environment.NewLine,
                    quantifierResult.QuantifierInput.Changes.Select(c =>
                        $"{c.FilePath} +{c.QuantifiedLinesAdded} -{c.QuantifiedLinesDeleted}"));

                Console.WriteLine(
                    $"PrQuantified = {quantifierResult.Label},\t" +
                    $"Diff +{quantifierResult.QuantifiedLinesAdded} -{quantifierResult.QuantifiedLinesDeleted} (Formula = {quantifierResult.Formula})," +
                    $"\tTeam percentiles: additions = {quantifierResult.PercentileAddition}%" +
                    $", deletions = {quantifierResult.PercentileDeletion}%.");
                Console.WriteLine(details);

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