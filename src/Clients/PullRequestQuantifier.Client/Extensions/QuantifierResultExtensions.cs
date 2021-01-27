namespace PullRequestQuantifier.Client.Extensions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Core;
    using Newtonsoft.Json;
    using Stubble.Core.Builders;

    public static class QuantifierResultExtensions
    {
        private const string FeedbackLinkRoot = "https://compliance.ppe.startclean.microsoft.com/api/feedback?payload=";

        public static async Task<string> ToMarkdownCommentAsync(
            this QuantifierResult quantifierResult,
            string repositoryLink,
            string pullRequestLink,
            string authorName)
        {
            var stubble = new StubbleBuilder()
                .Configure(settings => { settings.SetIgnoreCaseOnKeyLookup(true); }).Build();

            using var stream = new StreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    $"{typeof(QuantifierResultExtensions).Namespace}.QuantifierComment.mustache") !);

            var feedbackLinkThumbsUp = CreateFeedbackLink(
                "ThumbsUp",
                authorName,
                repositoryLink,
                pullRequestLink);
            var feedbackLinkNeutral = CreateFeedbackLink(
                "Neutral",
                authorName,
                repositoryLink,
                pullRequestLink);
            var feedbackLinkThumbsDown = CreateFeedbackLink(
                "ThumbsDown",
                authorName,
                repositoryLink,
                pullRequestLink);

            var comment = await stubble.RenderAsync(
                await stream.ReadToEndAsync(),
                new
                {
                    quantifierResult.Color,
                    quantifierResult.Label,
                    quantifierResult.QuantifiedLinesAdded,
                    quantifierResult.QuantifiedLinesDeleted,
                    quantifierResult.PercentileAddition,
                    quantifierResult.PercentileDeletion,
                    quantifierResult.FormulaPercentile,
                    Formula = quantifierResult.Formula.ToString(),
                    RepositoryLink = repositoryLink,
                    FeedbackLinkUp = feedbackLinkThumbsUp,
                    FeedbackLinkNeutral = feedbackLinkNeutral,
                    FeedbackLinkDown = feedbackLinkThumbsDown,
                    Details = string.Join(
                        Environment.NewLine,
                        quantifierResult.QuantifierInput.Changes
                            .GroupBy(c => c.FileExtension)
                            .Select(
                                g =>
                                    $"{g.Key} : +{g.Sum(v => v.QuantifiedLinesAdded)} -{g.Sum(v => v.QuantifiedLinesDeleted)}"))
                });
            return comment;
        }

        private static string CreateFeedbackLink(
            string eventType,
            string authorName,
            string repositoryLink,
            string pullRequestLink)
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

            return $"{FeedbackLinkRoot}{payload}";
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}