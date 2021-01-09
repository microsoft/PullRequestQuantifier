namespace PullRequestQuantifier.Client.QuantifyClient
***REMOVED***
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
    ***REMOVED***
        private readonly IPullRequestQuantifier prQuantifier;
        private readonly bool printJson;
        private readonly GitEngine gitEngine;
        private readonly QuantifyClientOutput quantifyClientOutput;

        public QuantifyClient(
            string contextFilePath,
            bool printJson,
            QuantifyClientOutput quantifyClientOutput)
        ***REMOVED***
            this.quantifyClientOutput = quantifyClientOutput;
            var context = LoadContext(contextFilePath);
            prQuantifier = new PullRequestQuantifier(context);
            this.printJson = printJson;
            gitEngine = new GitEngine();
***REMOVED***

        /// <inheritdoc />
        public Context Context => prQuantifier.Context;

        /// <inheritdoc />
        public async Task<QuantifierClientResult> Compute(string gitRepoPath)
        ***REMOVED***
            // get current location changes
            var quantifierInput = GetChanges(gitRepoPath);

            // quantify the changes
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            var quantifierClientResult = new QuantifierClientResult
            ***REMOVED***
                Color = quantifierResult.Color, Explanation = quantifierResult.Explanation,
                Formula = quantifierResult.Formula, QuantifiedLinesAdded = quantifierResult.QuantifiedLinesAdded,
                QuantifiedLinesDeleted = quantifierResult.QuantifiedLinesDeleted, Label = quantifierResult.Label,
                PercentileAddition = quantifierResult.PercentileAddition,
                PercentileDeletion = quantifierResult.PercentileDeletion,
                Details = Details(quantifierResult)
    ***REMOVED***;

            PrintQuantifierResult(quantifierClientResult);

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
            return quantifierClientResult;
***REMOVED***

        /// <inheritdoc />
        public async Task<QuantifierClientResult> Compute(QuantifierInput quantifierInput)
        ***REMOVED***
            // quantify the changes
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            var quantifierClientResult = new QuantifierClientResult
            ***REMOVED***
                Color = quantifierResult.Color, Explanation = quantifierResult.Explanation,
                Formula = quantifierResult.Formula, QuantifiedLinesAdded = quantifierResult.QuantifiedLinesAdded,
                QuantifiedLinesDeleted = quantifierResult.QuantifiedLinesDeleted, Label = quantifierResult.Label,
                PercentileAddition = quantifierResult.PercentileAddition,
                PercentileDeletion = quantifierResult.PercentileDeletion,
                Details = Details(quantifierResult)
    ***REMOVED***;

            PrintQuantifierResult(quantifierClientResult);

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
            return quantifierClientResult;
***REMOVED***

        private Context LoadContext(string contextFilePathOrContent)
        ***REMOVED***
            var context = DefaultContext.Value;
            if (string.IsNullOrEmpty(contextFilePathOrContent))
            ***REMOVED***
                return context;
    ***REMOVED***

            // if no valid context will be loaded then load the default one, don't fail
            try
            ***REMOVED***
                var ctx = ContextFactory.Load(contextFilePathOrContent);
                context = ctx;
    ***REMOVED***
            catch (YamlException)
            ***REMOVED***
    ***REMOVED***
            catch (ThresholdException ex)
            ***REMOVED***
                // misconfiguration then print the exception
                Console.WriteLine(ex);
    ***REMOVED***
            catch (ArgumentNullException ex)
            ***REMOVED***
                // misconfiguration then print the exception
                Console.WriteLine(ex);
    ***REMOVED***

            return context;
***REMOVED***

        private QuantifierInput GetChanges(string repoPath)
        ***REMOVED***
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(repoPath));

            return quantifierInput;
***REMOVED***

        private void PrintQuantifierResult(QuantifierClientResult quantifierClientResult)
        ***REMOVED***
            Console.WriteLine();
            if (printJson)
            ***REMOVED***
                Console.ForegroundColor = GetColor(quantifierClientResult.Color);

                Console.WriteLine(JsonSerializer.Serialize(
                    quantifierClientResult,
                    new JsonSerializerOptions ***REMOVED*** WriteIndented = true ***REMOVED***));

                Console.ResetColor();
    ***REMOVED***
            else
            ***REMOVED***
                Console.ForegroundColor = GetColor(quantifierClientResult.Color);

                var details = string.Join(
                    Environment.NewLine,
                    quantifierClientResult.Details.Select(v => $"***REMOVED***v.FilePath***REMOVED*** +***REMOVED***v.QuantifiedLinesAdded***REMOVED*** -***REMOVED***v.QuantifiedLinesDeleted***REMOVED***"));

                Console.WriteLine(
                    $"PrQuantified = ***REMOVED***quantifierClientResult.Label***REMOVED***,\t" +
                    $"Diff +***REMOVED***quantifierClientResult.QuantifiedLinesAdded***REMOVED*** -***REMOVED***quantifierClientResult.QuantifiedLinesDeleted***REMOVED*** (Formula = ***REMOVED***quantifierClientResult.Formula***REMOVED***)," +
                    $"\tTeam percentiles: additions = ***REMOVED***quantifierClientResult.PercentileAddition***REMOVED***%" +
                    $", deletions = ***REMOVED***quantifierClientResult.PercentileDeletion***REMOVED***%.");
                Console.WriteLine("PrQuantified details");
                Console.WriteLine(details);

                Console.ResetColor();
    ***REMOVED***
***REMOVED***

        private ConsoleColor GetColor(string color)
        ***REMOVED***
            return color switch
            ***REMOVED***
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
    ***REMOVED***;
***REMOVED***

        private IEnumerable<dynamic> Details(QuantifierResult quantifierResult)
        ***REMOVED***
            return quantifyClientOutput switch
            ***REMOVED***
                QuantifyClientOutput.SummaryByExt => quantifierResult.QuantifierInput.Changes.GroupBy(c => c.FileExtension).Select(g =>
                                         new
                                         ***REMOVED***
                                             FilePath = g.Key,
                                             FileExtension = g.Key,
                                             QuantifiedLinesAdded = g.Sum(v => v.QuantifiedLinesAdded),
                                             QuantifiedLinesDeleted = g.Sum(v => v.QuantifiedLinesDeleted),
                                             AbsoluteLinesAdded = g.Sum(v => v.AbsoluteLinesAdded),
                                             AbsoluteLinesDeleted = g.Sum(v => v.AbsoluteLinesDeleted)
                                 ***REMOVED***),
                QuantifyClientOutput.SummaryByFile => quantifierResult.QuantifierInput.Changes.Select(c => new
                ***REMOVED***
                    c.AbsoluteLinesAdded,
                    c.AbsoluteLinesDeleted,
                    c.FilePath,
                    c.QuantifiedLinesAdded,
                    c.QuantifiedLinesDeleted,
                    c.FileExtension
        ***REMOVED***),
                QuantifyClientOutput.Detailed => quantifierResult.QuantifierInput.Changes,
                _ => throw new ArgumentOutOfRangeException(),
    ***REMOVED***;
***REMOVED***
***REMOVED***
***REMOVED***