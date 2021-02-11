namespace PullRequestQuantifier.GitHub.Client.Tests.TestServer
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public static class ObjectExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(this object settings, string settingsRoot)
        {
            foreach (var property in settings.GetType().GetProperties())
            {
                yield return new KeyValuePair<string, string>($"{settingsRoot}:{property.Name}", property.GetValue(settings)?.ToString());
            }
        }
    }
}
