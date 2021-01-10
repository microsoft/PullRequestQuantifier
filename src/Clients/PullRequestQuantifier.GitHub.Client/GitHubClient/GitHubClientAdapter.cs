namespace PullRequestQuantifier.GitHub.Client.GitHubClient
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Octokit;

    /// <inheritdoc cref="IGitHubClientAdapter"/>
    public sealed class GitHubClientAdapter : IGitHubClientAdapter
    {
        // this value should be considered as non existing repo
        private const int NotInitializedRepositoryId = 0;
        private readonly Octokit.IGitHubClient gitHubClient;

        public GitHubClientAdapter(Octokit.IGitHubClient gitHubClient)
        {
            this.gitHubClient = gitHubClient;
        }

        /// <inheritdoc />
        public async Task<Repository> GetRepositoryByNameAsync(string organizationName, string repositoryName)
        {
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            return await gitHubClient.Repository.Get(organizationName, repositoryName);
        }

        /// <inheritdoc />
        public async Task<Repository> GetRepositoryByIdAsync(long repositoryId)
        {
            ArgumentCheck.IntegerIsGreaterThenThreshold(
                repositoryId,
                nameof(repositoryId),
                NotInitializedRepositoryId);
            return await gitHubClient.Repository.Get(repositoryId);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Repository>> GetRepositoriesForOrganizationAsync(string organizationName)
        {
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            return await gitHubClient.Repository.GetAllForOrg(organizationName);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<GitHubCommit>> GetCommitsAsync(
            string organizationName,
            string repositoryName,
            string path = "")
        {
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            return await gitHubClient.Repository.Commit.GetAll(
                organizationName,
                repositoryName,
                new CommitRequest { Path = path });
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RepositoryContent>> GetRepositoryContentByRefAsync(
            string organizationName,
            string repositoryName,
            string path,
            string reference)
        {
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(path, nameof(path));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(reference, nameof(reference));
            return await gitHubClient.Repository.Content.GetAllContentsByRef(
                organizationName,
                repositoryName,
                path,
                reference);
        }

        /// <inheritdoc />
        public async Task<TreeResponse> GetGitTreeRecursiveAsync(
            string organizationName,
            string repositoryName,
            string reference)
        {
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(reference, nameof(reference));
            return await gitHubClient.Git.Tree.GetRecursive(
                organizationName,
                repositoryName,
                reference);
        }

        /// <inheritdoc />
        public async Task<Blob> GetGitBlobAsync(
            string organizationName,
            string repositoryName,
            string reference)
        {
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(reference, nameof(reference));
            return await gitHubClient.Git.Blob.Get(
                organizationName,
                repositoryName,
                reference);
        }

        /// <inheritdoc />
        public async Task<RepositoryContentChangeSet> CreateFileAsync(
            string organizationName,
            string repositoryName,
            string path,
            CreateFileRequest createFileRequest)
        {
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(path, nameof(path));

            return await gitHubClient.Repository.Content.CreateFile(
                organizationName,
                repositoryName,
                path,
                createFileRequest);
        }

        /// <inheritdoc />
        public async Task DeleteFileAsync(
            string organizationName,
            string repositoryName,
            string path,
            DeleteFileRequest deleteFileRequest)
        {
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(path, nameof(path));

            await gitHubClient.Repository.Content.DeleteFile(
                organizationName,
                repositoryName,
                path,
                deleteFileRequest);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<PullRequestFile>> GetPullRequestFiles(long repositoryId, int pullRequestNumber)
        {
            return await gitHubClient.PullRequest.Files(repositoryId, pullRequestNumber);
        }
    }
}