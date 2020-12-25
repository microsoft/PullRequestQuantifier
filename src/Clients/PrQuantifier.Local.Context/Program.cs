namespace PrQuantifier.Local.Context
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Abstractions.Context;
    using GitEngine;
    using global::PrQuantifier.Client;

    public static class Program
    {
        private static readonly IGitEngine GitEngine = new GitEngine();

        public static async Task Main(string[] args)
        {
            args = CheckArgs(args);

            IContextGenerator contextGenerator = new ContextGenerator();
            var context = await contextGenerator.Create(args[0]);

            // serialize the new context
            var repoRootPath = new DirectoryInfo(GitEngine.GetRepoRoot(args[0])).Parent.FullName;
            var filePath = Path.Combine(repoRootPath, ".prquantifier");
            context.SerializeToYaml(filePath);
            Console.WriteLine(
                $"Generate context for Repo located on '{repoRootPath}'" +
                $", context file located at {Path.Combine(Environment.CurrentDirectory, filePath)}");
        }

        private static string[] CheckArgs(string[] args)
        {
            // if repo path is missing then consider the local repo
            if (args == null || args.Length == 0)
            {
                return new[] { Environment.CurrentDirectory };
            }

            if (!Directory.Exists(args[0]))
            {
                throw new DirectoryNotFoundException(args[0]);
            }

            return args;
        }
    }
}