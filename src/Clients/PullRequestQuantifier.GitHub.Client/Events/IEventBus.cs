namespace PullRequestQuantifier.GitHub.Client.Events
{
    using Newtonsoft.Json.Linq;

    public interface IEventBus
    {
        void Write(JObject payload);

        JObject ReadNext();
    }
}
