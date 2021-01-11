namespace PullRequestQuantifier.Client.QuantifyClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;
    using global::PullRequestQuantifier.Abstractions.Exceptions;
    using global::PullRequestQuantifier.Client.ContextGenerator;
    using global::PullRequestQuantifier.Client.Helpers;
    using global::PullRequestQuantifier.GitEngine;
    using YamlDotNet.Core;

    /// <inheritdoc />
    public sealed class QuantifyClient : IQuantifyClient
    {
        private readonly IPullRequestQuantifier prQuantifier;
        private readonly GitEngine gitEngine;
        private readonly QuantifyClientOutput quantifyClientOutput;
        private readonly bool print;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuantifyClient"/> class.
        /// </summary>
        /// <param name="contextFilePath">The path of the context file.</param>
        /// <param name="quantifyClientOutput">The output type.</param>
        /// <param name="print">If the result should be printed out to console(for now) or not.
        /// This is necessary for Vsix extension which consumes directly the local client.</param>
        public QuantifyClient(
            string contextFilePath,
            QuantifyClientOutput quantifyClientOutput,
            bool print = true)
        {
            this.print = print;
            this.quantifyClientOutput = quantifyClientOutput;
            var context = LoadContext(contextFilePath);
            prQuantifier = new PullRequestQuantifier(context);
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

            if (print)
            {
                PrintQuantifierResult(quantifierClientResult);
            }

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

            if (print)
            {
                PrintQuantifierResult(quantifierClientResult);
            }

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
            Console.ForegroundColor = QuantifyClientHelper.GetColor(quantifierClientResult.Color);

            Console.WriteLine(JsonSerializer.Serialize(
                quantifierClientResult,
                new JsonSerializerOptions { WriteIndented = true }));

            Console.ResetColor();
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