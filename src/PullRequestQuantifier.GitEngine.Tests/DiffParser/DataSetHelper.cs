namespace PullRequestQuantifier.GitEngine.Tests.DiffParser
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;

    [ExcludeFromCodeCoverage]
    public class DataSetHelper
    {
        public static string ReadFileContent(string dataSetId, string filename)
        {
            var fileSystem = new FileSystem();
            var fileContent = fileSystem.File.ReadAllText($"DiffParser/Data/{dataSetId}/{filename}");
            return fileContent;
        }
    }
}
