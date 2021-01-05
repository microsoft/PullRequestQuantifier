namespace PullRequestQuantifier.Client
***REMOVED***
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;
    using global::PullRequestQuantifier.Abstractions.Exceptions;
    using global::PullRequestQuantifier.GitEngine;
    using YamlDotNet.Core;

    /// <inheritdoc />
    public sealed class QuantifyClient : IQuantifyClient
    ***REMOVED***
        private readonly IPullRequestQuantifier prQuantifier;
        private readonly bool printJson;
        private readonly GitEngine gitEngine;

        public QuantifyClient(
            string contextFilePath,
            bool printJson)
        ***REMOVED***
            var context = LoadContext(contextFilePath);
            prQuantifier = new PullRequestQuantifier(context);
            this.printJson = printJson;
            gitEngine = new GitEngine();
***REMOVED***

        /// <inheritdoc />
        public Context Context => prQuantifier.Context;

        /// <inheritdoc />
        public async Task<QuantifierResult> Compute(string gitRepoPath)
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

        private void PrintQuantifierResult(QuantifierResult quantifierResult)
        ***REMOVED***
            if (printJson)
            ***REMOVED***
                Console.WriteLine(JsonSerializer.Serialize(
                    quantifierResult,
                    new JsonSerializerOptions ***REMOVED*** WriteIndented = true ***REMOVED***));
    ***REMOVED***
            else
            ***REMOVED***
                Console.ForegroundColor = GetColor(quantifierResult.Color);
                Console.WriteLine(
                    $"PrQuantified = ***REMOVED***quantifierResult.Label***REMOVED***,\t" +
                    $"Diff +***REMOVED***quantifierResult.QuantifiedLinesAdded***REMOVED*** -***REMOVED***quantifierResult.QuantifiedLinesDeleted***REMOVED*** (Formula = ***REMOVED***quantifierResult.Formula***REMOVED***)," +
                    $"\tTeam percentiles: additions = ***REMOVED***quantifierResult.PercentileAddition***REMOVED***%" +
                    $", deletions = ***REMOVED***quantifierResult.PercentileDeletion***REMOVED***%.");
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
***REMOVED***
***REMOVED***