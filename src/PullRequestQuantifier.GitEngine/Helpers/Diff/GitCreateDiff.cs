namespace PullRequestQuantifier.GitEngine.Helpers.Diff
{
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using LibGit2Sharp;
    using PullRequestQuantifier.Abstractions.Git;

    public sealed class GitCreateDiff : IDisposable
    {
        private readonly IGitEngine gitEngine;
        private string repoPath;
        private FileSystem fileSystem;

        public GitCreateDiff()
        {
            gitEngine = new GitEngine();
            CreateGitRepo();
        }

        /// <summary>
        /// Create a diff git patch format out of the contents,
        /// one was the file content before modification and the other the modified content.
        /// </summary>
        /// <param name="baseContent">Base file content, the unmodified one.</param>
        /// <param name="compareContent">The modified file content.</param>
        /// <returns>returns a GitFilePatch.</returns>
        public GitFilePatch CreateDiff(
            string baseContent,
            string compareContent)
        {
            string fileName = Guid.NewGuid().ToString();

            fileSystem.File.WriteAllText(
                fileSystem.Path.Combine(repoPath, fileName),
                baseContent);

            Commit();

            fileSystem.File.WriteAllText(
                fileSystem.Path.Combine(repoPath, fileName),
                compareContent);

            return gitEngine.GetGitChanges(repoPath).FirstOrDefault(c =>
                c.FilePath.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Dispose()
        {
            DeleteRepoDirectory();
        }

        private void CreateGitRepo()
        {
            // Setup directory for git
            fileSystem = new FileSystem();
            repoPath = fileSystem.Path.Combine(
                fileSystem.Path.GetTempPath(),
                Guid.NewGuid().ToString());

            fileSystem = gitEngine.CreateRepository(repoPath);
        }

        private void Commit()
        {
            var repository = new Repository(repoPath);
            Commands.Stage(repository, "*");
            var author = new Signature("user", "email", DateTimeOffset.Now);
            repository.Commit("Adding files", author, author);
        }

        // This implementation of delete directory is based on the stack overflow
        // answer https://stackoverflow.com/questions/1701457/directory-delete-doesnt-work-access-denied-error-but-under-windows-explorer-it.
        // Otherwise this runs into access issues during direct deletion sometimes.
        private void DeleteRepoDirectory()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var dirInfo = fileSystem.DirectoryInfo.New(repoPath);
#pragma warning restore CS0618 // Type or member is obsolete
            if (dirInfo.Exists)
            {
                try
                {
                    SetNormalAttribute(dirInfo);
                    dirInfo.Delete(true);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void SetNormalAttribute(IDirectoryInfo dirInfo)
        {
            if (!dirInfo.Exists)
            {
                return;
            }

            var directories = dirInfo.GetDirectories().ToArray();
            foreach (var subDir in directories)
            {
                SetNormalAttribute(subDir);
            }

            var files = dirInfo.GetFiles().ToArray();
            foreach (var file in files)
            {
                file.Attributes = FileAttributes.Normal;
            }
        }
    }
}
