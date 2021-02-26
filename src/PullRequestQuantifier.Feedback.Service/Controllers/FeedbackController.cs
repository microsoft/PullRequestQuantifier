namespace PullRequestQuantifier.Feedback.Service.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;
    using PullRequestQuantifier.Common.Azure.BlobStorage;
    using PullRequestQuantifier.Feedback.Service.Models;

    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class FeedbackController : ControllerBase
    {
        private readonly IBlobStorage blobStorage;
        private readonly ILogger<FeedbackController> logger;
        private readonly FeedbackForm feedbackFormSettings;

        public FeedbackController(
            IBlobStorage blobStorage,
            ILogger<FeedbackController> logger,
            IOptions<FeedbackForm> feedbackFormSettings)
        {
            this.blobStorage = blobStorage;
            this.logger = logger;
            this.feedbackFormSettings = feedbackFormSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Feedback(string payload, bool anonymous = true)
        {
            if (string.IsNullOrEmpty(payload))
            {
                return Ok($"Something went wrong, {nameof(payload)} is empty!");
            }

            try
            {
                var decodedJsonPayload = Base64Decode(payload);
                var jObject = JObject.Parse(decodedJsonPayload);

                var feedbackModel = new FeedbackModel(
                    Base64Encode(jObject["AuthorName"].Value<string>()),
                    Base64Encode(Guid.NewGuid().ToString()))
                {
                    Text = jObject["PullRequestLink"].Value<string>(),
                    EventType = jObject["EventType"].Value<string>(),
                    NotNormalizedPartitionKey = jObject["AuthorName"].Value<string>()
                };

                await blobStorage.InsertOrReplaceTableEntityAsync(
                    nameof(FeedbackModel),
                    feedbackModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(FeedbackController));
            }

            var feedbackFormUrl = feedbackFormSettings.NonAnonymousFormUrl;
            if (anonymous)
            {
                feedbackFormUrl = feedbackFormSettings.AnonymousFormUrl;
            }

            return Redirect(feedbackFormUrl);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
