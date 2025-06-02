namespace PullRequestQuantifier.Tools.CloneAdoRepo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using LibGit2Sharp;
    using PullRequestQuantifier.Tools.Common;
    using PullRequestQuantifier.Tools.Common.Model;
    using YamlDotNet.Serialization;
    using Repository = LibGit2Sharp.Repository;

    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var commandLine = new CommandLine(args);

            var organizations = new DeserializerBuilder().Build().Deserialize<List<Organization>>(await File.ReadAllTextAsync(commandLine.ConfigFile));
            await Clone(
                organizations,
                commandLine.ClonePath,
                commandLine.User,
                commandLine.Pat);
        }

        private static async Task Clone(
            IEnumerable<Organization> organizations,
            string clonePath,
            string userName,
            string pat)
        {
            var credentials = new NetworkCredential(userName, pat);
            var cloneOptions = new CloneOptions();
            cloneOptions.FetchOptions.CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
            {
                Username = credentials.UserName,
                Password = credentials.Password
            };

            foreach (var organization in organizations)
            {
                foreach (var project in organization.Projects)
                {
                    foreach (var repository in project.Repositories)
                    {
                        var path = Path.Combine(clonePath, repository.Name);
                        var repoRoot = Repository.Discover(path);

                        // don't crash when there is no repo to this path, return empty changes
                        if (repoRoot != null)
                        {
                            Console.WriteLine($"Repo {repository.Name} already exists! Continue with other.");
                            continue;
                        }

                        Console.WriteLine($"Cloning {repository.Name} repository!");
                        Directory.CreateDirectory(path);
                        await Task.Run(() => Repository.Clone(
                            $"https://{Uri.EscapeDataString(organization.Name)}@dev.azure.com/{Uri.EscapeDataString(organization.Name)}/" +
                            $"{Uri.EscapeDataString(project.Name)}/_git/{Uri.EscapeDataString(repository.Name)}",
                            path,
                            cloneOptions));
                    }
                }
            }
        }
    }
}
