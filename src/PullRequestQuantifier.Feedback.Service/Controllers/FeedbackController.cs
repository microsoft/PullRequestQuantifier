namespace PullRequestQuantifier.Feedback.Service.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using PullRequestQuantifier.Common.Azure.BlobStorage;
    using PullRequestQuantifier.Feedback.Service.Models;

    // todo move this to a separate service if the new feedback is going well.
    // todo The only reason to place it here for now is to do it have a fast deployment for the new MerlinBot feedback feature.
    // don't deal for now with Auth, we only experiment, in case is working then we can use azure API, which provides a key based auth system
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IBlobStorage blobStorage;
        private readonly ILogger<FeedbackController> logger;

        public FeedbackController(
            IBlobStorage blobStorage,
            ILogger<FeedbackController> logger)
        {
            this.blobStorage = blobStorage;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Feedback(string payload)
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

            return Ok("Thanks for your feedback!");
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
