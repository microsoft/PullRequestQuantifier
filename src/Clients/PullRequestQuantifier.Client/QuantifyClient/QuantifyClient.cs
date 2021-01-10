namespace PullRequestQuantifier.Client.QuantifyClient
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;
    using global::PullRequestQuantifier.Abstractions.Exceptions;
    using global::PullRequestQuantifier.Client.ContextGenerator;
    using global::PullRequestQuantifier.GitEngine;
    using YamlDotNet.Core;

    /// <inheritdoc />
    public sealed class QuantifyClient : IQuantifyClient
    {
        private readonly IPullRequestQuantifier prQuantifier;
        private readonly bool printJson;
        private readonly GitEngine gitEngine;
        private readonly QuantifyClientOutput quantifyClientOutput;

        public QuantifyClient(
            string contextFilePath,
            bool printJson,
            QuantifyClientOutput quantifyClientOutput)
        {
            this.quantifyClientOutput = quantifyClientOutput;
            var context = LoadContext(contextFilePath);
            prQuantifier = new PullRequestQuantifier(context);
            this.printJson = printJson;
            gitEngine = new GitEngine();
        }

        /// <inheritdoc />
        public Context Context => prQuantifier.Context;

        /// <inheritdoc />
        public async Task<QuantifierClientResult> Compute(string gitRepoPath)
        {
            // get current location changes
            var quantifierInput = GetChanges(gitRepoPath);

            // quantify the changes
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            var quantifierClientResult = new QuantifierClientResult
            {
                Color = quantifierResult.Color, Explanation = quantifierResult.Explanation,
                Formula = quantifierResult.Formula, QuantifiedLinesAdded = quantifierResult.QuantifiedLinesAdded,
                QuantifiedLinesDeleted = quantifierResult.QuantifiedLinesDeleted, Label = quantifierResult.Label,
                PercentileAddition = quantifierResult.PercentileAddition,
                PercentileDeletion = quantifierResult.PercentileDeletion,
                Details = Details(quantifierResult)
            };

            PrintQuantifierResult(quantifierClientResult);

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
            return quantifierClientResult;
        }

        /// <inheritdoc />
        public async Task<QuantifierClientResult> Compute(QuantifierInput quantifierInput)
        {
            // quantify the changes
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            var quantifierClientResult = new QuantifierClientResult
            {
                Color = quantifierResult.Color, Explanation = quantifierResult.Explanation,
                Formula = quantifierResult.Formula, QuantifiedLinesAdded = quantifierResult.QuantifiedLinesAdded,
                QuantifiedLinesDeleted = quantifierResult.QuantifiedLinesDeleted, Label = quantifierResult.Label,
                PercentileAddition = quantifierResult.PercentileAddition,
                PercentileDeletion = quantifierResult.PercentileDeletion,
                Details = Details(quantifierResult)
            };

            PrintQuantifierResult(quantifierClientResult);

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
            return quantifierClientResult;
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

        private void PrintQuantifierResult(QuantifierClientResult quantifierClientResult)
        {
            Console.WriteLine();
            if (printJson)
            {
                Console.ForegroundColor = GetColor(quantifierClientResult.Color);

                Console.WriteLine(JsonSerializer.Serialize(
                    quantifierClientResult,
                    new JsonSerializerOptions { WriteIndented = true }));

                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = GetColor(quantifierClientResult.Color);

                var details = string.Join(
                    Environment.NewLine,
                    quantifierClientResult.Details.Select(v => $"{v.FilePath} +{v.QuantifiedLinesAdded} -{v.QuantifiedLinesDeleted}"));

                Console.WriteLine(
                    $"PrQuantified = {quantifierClientResult.Label},\t" +
                    $"Diff +{quantifierClientResult.QuantifiedLinesAdded} -{quantifierClientResult.QuantifiedLinesDeleted} (Formula = {quantifierClientResult.Formula})," +
                    $"\tTeam percentiles: additions = {quantifierClientResult.PercentileAddition}%" +
                    $", deletions = {quantifierClientResult.PercentileDeletion}%.");
                Console.WriteLine("PrQuantified details");
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

        private IEnumerable<dynamic> Details(QuantifierResult quantifierResult)
        {
            return quantifyClientOutput switch
            {
                QuantifyClientOutput.SummaryByExt => quantifierResult.QuantifierInput.Changes.GroupBy(c => c.FileExtension).Select(g =>
                                         new
                                         {
                                             FilePath = g.Key,
                                             FileExtension = g.Key,
                                             QuantifiedLinesAdded = g.Sum(v => v.QuantifiedLinesAdded),
                                             QuantifiedLinesDeleted = g.Sum(v => v.QuantifiedLinesDeleted),
                                             AbsoluteLinesAdded = g.Sum(v => v.AbsoluteLinesAdded),
                                             AbsoluteLinesDeleted = g.Sum(v => v.AbsoluteLinesDeleted)
                                         }),
                QuantifyClientOutput.SummaryByFile => quantifierResult.QuantifierInput.Changes.Select(c => new
                {
                    c.AbsoluteLinesAdded,
                    c.AbsoluteLinesDeleted,
                    c.FilePath,
                    c.QuantifiedLinesAdded,
                    c.QuantifiedLinesDeleted,
                    c.FileExtension
                }),
                QuantifyClientOutput.Detailed => quantifierResult.QuantifierInput.Changes,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}