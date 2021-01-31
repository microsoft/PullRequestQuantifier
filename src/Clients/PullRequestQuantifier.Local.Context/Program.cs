namespace PullRequestQuantifier.Local.Context
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using LibGit2Sharp;
    using PullRequestQuantifier.Client.ContextGenerator;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            args = CheckArgs(args);

            IContextGenerator contextGenerator = new ContextGenerator();
            var context = await contextGenerator.Create(args[0]);

            var repository = Repository.Discover(args[0]);
            var saveContextFilePath = args[0];
            if (repository != null)
            {
                saveContextFilePath = new DirectoryInfo(repository).Parent.FullName;
            }

            // serialize the new context
            var filePath = Path.Combine(saveContextFilePath, "prquantifier.yaml");
            context.SerializeToYaml(filePath);
            Console.WriteLine(
                $"Generate context for Repo located on '{saveContextFilePath}'" +
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