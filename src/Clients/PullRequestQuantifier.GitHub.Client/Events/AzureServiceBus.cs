namespace PullRequestQuantifier.GitHub.Client.Events
{
    using Newtonsoft.Json.Linq;

    public class AzureServiceBus : IEventBus
    {
        public void Write(JObject payload)
        {
            throw new System.NotImplementedException();
        }

        public JObject ReadNext()
        {
            throw new System.NotImplementedException();
        }
    }
}
