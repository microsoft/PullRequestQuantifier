namespace PullRequestQuantifier.Client.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Core;
    using Newtonsoft.Json;
    using Stubble.Core.Builders;

    public static class QuantifierResultExtensions
    {
        private const string FeedbackLinkRoot = "https://pullrequestquantifierfeedback.azurewebsites.net/feedback?payload=";

        public static async Task<string> ToConsoleOutput(this QuantifierResult quantifierResult)
        {
            var stubble = new StubbleBuilder()
                .Configure(settings => { settings.SetIgnoreCaseOnKeyLookup(true); }).Build();

            using var stream = new StreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    $"{typeof(QuantifierResultExtensions).Namespace}.ConsoleOutput.mustache") !);

            var consoleOutput = await stubble.RenderAsync(
                await stream.ReadToEndAsync(),
                new
                {
                    quantifierResult.Label,
                    quantifierResult.QuantifiedLinesAdded,
                    quantifierResult.QuantifiedLinesDeleted,
                    quantifierResult.PercentileAddition,
                    quantifierResult.PercentileDeletion,
                    quantifierResult.FormulaPercentile,
                    Formula = quantifierResult.Formula.ToString(),
                });
            return consoleOutput;
        }

        public static async Task<string> ToMarkdownCommentAsync(
            this QuantifierResult quantifierResult,
            string repositoryLink,
            string contextFileLink,
            string pullRequestLink,
            string authorName,
            bool anonymous = true,
            MarkdownCommentOptions markdownCommentOptions = null)
        {
            markdownCommentOptions ??= new MarkdownCommentOptions();

            var stubble = new StubbleBuilder()
                .Configure(settings => { settings.SetIgnoreCaseOnKeyLookup(true); }).Build();

            using var stream = new StreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    $"{typeof(QuantifierResultExtensions).Namespace}.QuantifierComment.mustache") !);

            var feedbackLinkThumbsUp = CreateFeedbackLink(
                "ThumbsUp",
                authorName,
                repositoryLink,
                pullRequestLink,
                anonymous);
            var feedbackLinkNeutral = CreateFeedbackLink(
                "Neutral",
                authorName,
                repositoryLink,
                pullRequestLink,
                anonymous);
            var feedbackLinkThumbsDown = CreateFeedbackLink(
                "ThumbsDown",
                authorName,
                repositoryLink,
                pullRequestLink,
                anonymous);

            var contextFormulaPercentile =
                quantifierResult.Context.FormulaPercentile.First(f => f.Item1 == quantifierResult.Formula).Item2;
            var (idealSizeLowerBound, idealSizeUpperBound) = GetIdealChangeCountRange(contextFormulaPercentile);
            var detailsByFileExt = quantifierResult.QuantifierInput.Changes
                .Where(c => !string.IsNullOrEmpty(c.FileExtension))
                .GroupBy(c => c.FileExtension)
                .Select(
                    g =>
                        $"{g.Key} : +{g.Sum(v => v.QuantifiedLinesAdded)} -{g.Sum(v => v.QuantifiedLinesDeleted)}")
                .Concat(
                    quantifierResult.QuantifierInput.Changes
                        .Where(c => string.IsNullOrEmpty(c.FileExtension))
                        .Select(c => $"{c.FilePath} : +{c.QuantifiedLinesAdded} -{c.QuantifiedLinesDeleted}"));
            var comment = await stubble.RenderAsync(
                await stream.ReadToEndAsync(),
                new
                {
                    quantifierResult.Color,
                    quantifierResult.Label,
                    quantifierResult.QuantifiedLinesAdded,
                    quantifierResult.QuantifiedLinesDeleted,
                    quantifierResult.FormulaLinesChanged,
                    quantifierResult.PercentileAddition,
                    quantifierResult.PercentileDeletion,
                    quantifierResult.FormulaPercentile,
                    IdealSizeLowerBound = idealSizeLowerBound,
                    IdealSizeUpperBound = idealSizeUpperBound,
                    IsIdealSize = quantifierResult.FormulaLinesChanged >= idealSizeLowerBound && quantifierResult.FormulaLinesChanged <= idealSizeUpperBound,
                    Formula = quantifierResult.Formula.ToString(),
                    ContextFileLink = contextFileLink,
                    FeedbackLinkUp = feedbackLinkThumbsUp,
                    FeedbackLinkNeutral = feedbackLinkNeutral,
                    FeedbackLinkDown = feedbackLinkThumbsDown,
                    TotalFilesChanged = quantifierResult.QuantifierInput.Changes.Count,
                    Details = string.Join(Environment.NewLine, detailsByFileExt),
                    CollapsePullRequestQuantifiedSection = markdownCommentOptions.CollapsePullRequestQuantifiedSection ? string.Empty : "open"
                });
            return comment;
        }

        private static (int, int) GetIdealChangeCountRange(SortedDictionary<int, float> contextPercentile)
        {
            float idealLowerPercentile = 20;
            float idealUpperPercentile = 66;

            var percentilesArray = contextPercentile.Values.ToArray();
            var changeCountsArray = contextPercentile.Keys.ToArray();

            var idxUpperBound = Array.FindIndex(
                percentilesArray,
                arrayElement => arrayElement >= idealUpperPercentile);

            var idxLowerBound = Array.FindLastIndex(
                percentilesArray,
                arrayElement => arrayElement <= idealLowerPercentile);

            var lowerBoundChangeCount =
                idealLowerPercentile < percentilesArray[0] ? changeCountsArray[0] : changeCountsArray[idxLowerBound];

            var upperBoundChangeCount =
                idealUpperPercentile > percentilesArray[^1] ? changeCountsArray[^1] : changeCountsArray[idxUpperBound];

            return (lowerBoundChangeCount, upperBoundChangeCount);
        }

        private static string CreateFeedbackLink(
            string eventType,
            string authorName,
            string repositoryLink,
            string pullRequestLink,
            bool anonymous)
        {
            var payload = Base64Encode(
                JsonConvert.SerializeObject(
                    new
                    {
                        AuthorName = authorName,
                        RepositoryLink = repositoryLink,
                        PullRequestLink = pullRequestLink,
                        EventType = eventType
                    }));

            return $"{FeedbackLinkRoot}{payload}&anonymous={anonymous}";
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
