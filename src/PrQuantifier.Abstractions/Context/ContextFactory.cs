namespace PrQuantifier.Abstractions.Context
{
    using System.IO;
    using YamlDotNet.Serialization;

    public static class ContextFactory
    {
        public static Context Load(string filePathOrContent)
        {
            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();

            var contentYml = File.Exists(filePathOrContent)
                ? File.ReadAllText(filePathOrContent)
                : filePathOrContent;

            var context = deserializer.Deserialize<Context>(contentYml);

            return context.Validate();
        }
    }
}
