namespace PrQuantifier.Abstractions.Context
***REMOVED***
    using System.IO;
    using YamlDotNet.Serialization;

    public static class ContextFactory
    ***REMOVED***
        public static Context Load(string filePathOrContent)
        ***REMOVED***
            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();

            var contentYml = File.Exists(filePathOrContent)
                ? File.ReadAllText(filePathOrContent)
                : filePathOrContent;

            var context = deserializer.Deserialize<Context>(contentYml);

            return context.Validate();
***REMOVED***
***REMOVED***
***REMOVED***
