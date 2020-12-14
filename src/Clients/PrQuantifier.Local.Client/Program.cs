namespace PrQuantifier.Local.Client
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using global::PrQuantifier.Core.Context;
    using global::PrQuantifier.Core.Git;

    public static class Program
    {
        private const string PrQuantifierResults = "PrQuantifierResults";

        public static async Task Main(string[] args)
        {
            CheckArgs(args);

            // get current location changes
            var quantifierInput = GetChanges(Environment.CurrentDirectory);

            // quantify the changes
            var prQuantifier = new PrQuantifier(ContextFactory.Load(args[0]));
            var quantifierResult = await prQuantifier.Quantify(quantifierInput);

            // write the results
            var quantifierResultJson = JsonSerializer.Serialize(quantifierResult);

            var outputFilePath = Path.Combine(Environment.CurrentDirectory, PrQuantifierResults, $"{Guid.NewGuid()}.prQuantifier.json");
            File.WriteAllText(
                outputFilePath,
                quantifierResultJson);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Label = {quantifierResult.Label}\tDiff +{quantifierResult.QuantifiedLinesAdded} -{quantifierResult.QuantifiedLinesDeleted}");
            Console.ResetColor();
            Console.WriteLine($"More details here: {outputFilePath}");

            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
        }

        private static QuantifierInput GetChanges(string repoPath)
        {
            var gitEngine = new GitEngine();
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(repoPath));

            return quantifierInput;
        }

        private static void CheckArgs(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new ArgumentException("Context model file path is missing!");
            }

            if (!File.Exists(args[0]))
            {
                throw new FileNotFoundException(args[0]);
            }

            Directory.CreateDirectory(PrQuantifierResults);
        }
    }
}
