namespace PullRequestQuantifier.GitHub.Client
***REMOVED***
    using System.Net.Http;
    using System.Threading.Tasks;
    using GitHubJwt;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Octokit;
    using PullRequestQuantifier.GitHub.Client.GitHubClient;
    using PullRequestQuantifier.GitHub.Client.Telemetry;

    public class QuantifierFunction
    ***REMOVED***
        private readonly IGitHubJwtFactory gitHubJwtFactory;

        private readonly GitHubAppSettings gitHubAppSettings;

        private readonly IAppTelemetry appTelemetry;

        public QuantifierFunction(
            IGitHubJwtFactory gitHubJwtFactory,
            IOptions<GitHubAppSettings> gitHubAppSettings,
            IAppTelemetry appTelemetry)
        ***REMOVED***
            this.gitHubJwtFactory = gitHubJwtFactory;
            this.appTelemetry = appTelemetry;
            this.gitHubAppSettings = gitHubAppSettings.Value;
***REMOVED***

        [FunctionName("quantify")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log)
        ***REMOVED***
            var gitHubClientFactory = GitHubClientFactory.Create(
                "org",
                new Credentials(gitHubJwtFactory.CreateEncodedJwtToken(), AuthenticationType.Bearer),
                gitHubAppSettings,
                appTelemetry);
            await gitHubClientFactory.GetGitHubGitClientAsync();
            return new OkObjectResult("Response from function with injected dependencies.");
***REMOVED***
***REMOVED***
***REMOVED***
