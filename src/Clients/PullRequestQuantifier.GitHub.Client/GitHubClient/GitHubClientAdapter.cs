namespace PullRequestQuantifier.GitHub.Client.GitHubClient
***REMOVED***
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Octokit;

    /// <inheritdoc cref="IGitHubClientAdapter"/>
    public sealed class GitHubClientAdapter : IGitHubClientAdapter
    ***REMOVED***
        // this value should be considered as non existing repo
        private const int NotInitializedRepositoryId = 0;
        private readonly Octokit.IGitHubClient gitHubClient;

        public GitHubClientAdapter(Octokit.IGitHubClient gitHubClient)
        ***REMOVED***
            this.gitHubClient = gitHubClient;
***REMOVED***

        /// <inheritdoc />
        public async Task<Repository> GetRepositoryByNameAsync(string organizationName, string repositoryName)
        ***REMOVED***
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            return await gitHubClient.Repository.Get(organizationName, repositoryName);
***REMOVED***

        /// <inheritdoc />
        public async Task<Repository> GetRepositoryByIdAsync(long repositoryId)
        ***REMOVED***
            ArgumentCheck.IntegerIsGreaterThenThreshold(
                repositoryId,
                nameof(repositoryId),
                NotInitializedRepositoryId);
            return await gitHubClient.Repository.Get(repositoryId);
***REMOVED***

        /// <inheritdoc />
        public async Task<IReadOnlyList<Repository>> GetRepositoriesForOrganizationAsync(string organizationName)
        ***REMOVED***
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            return await gitHubClient.Repository.GetAllForOrg(organizationName);
***REMOVED***

        /// <inheritdoc />
        public async Task<IReadOnlyList<GitHubCommit>> GetCommitsAsync(
            string organizationName,
            string repositoryName,
            string path = "")
        ***REMOVED***
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            return await gitHubClient.Repository.Commit.GetAll(
                organizationName,
                repositoryName,
                new CommitRequest ***REMOVED*** Path = path ***REMOVED***);
***REMOVED***

        /// <inheritdoc />
        public async Task<IReadOnlyList<RepositoryContent>> GetRepositoryContentByRefAsync(
            string organizationName,
            string repositoryName,
            string path,
            string reference)
        ***REMOVED***
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(path, nameof(path));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(reference, nameof(reference));
            return await gitHubClient.Repository.Content.GetAllContentsByRef(
                organizationName,
                repositoryName,
                path,
                reference);
***REMOVED***

        /// <inheritdoc />
        public async Task<TreeResponse> GetGitTreeRecursiveAsync(
            string organizationName,
            string repositoryName,
            string reference)
        ***REMOVED***
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(reference, nameof(reference));
            return await gitHubClient.Git.Tree.GetRecursive(
                organizationName,
                repositoryName,
                reference);
***REMOVED***

        /// <inheritdoc />
        public async Task<Blob> GetGitBlobAsync(
            string organizationName,
            string repositoryName,
            string reference)
        ***REMOVED***
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(reference, nameof(reference));
            return await gitHubClient.Git.Blob.Get(
                organizationName,
                repositoryName,
                reference);
***REMOVED***

        /// <inheritdoc />
        public async Task<RepositoryContentChangeSet> CreateFileAsync(
            string organizationName,
            string repositoryName,
            string path,
            CreateFileRequest createFileRequest)
        ***REMOVED***
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(path, nameof(path));

            return await gitHubClient.Repository.Content.CreateFile(
                organizationName,
                repositoryName,
                path,
                createFileRequest);
***REMOVED***

        /// <inheritdoc />
        public async Task DeleteFileAsync(
            string organizationName,
            string repositoryName,
            string path,
            DeleteFileRequest deleteFileRequest)
        ***REMOVED***
            ArgumentCheck.StringIsNotNullOrWhiteSpace(organizationName, nameof(organizationName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(repositoryName, nameof(repositoryName));
            ArgumentCheck.StringIsNotNullOrWhiteSpace(path, nameof(path));

            await gitHubClient.Repository.Content.DeleteFile(
                organizationName,
                repositoryName,
                path,
                deleteFileRequest);
***REMOVED***

        /// <inheritdoc />
        public async Task<IReadOnlyList<PullRequestFile>> GetPullRequestFiles(long repositoryId, int pullRequestNumber)
        ***REMOVED***
            return await gitHubClient.PullRequest.Files(repositoryId, pullRequestNumber);
***REMOVED***
***REMOVED***
***REMOVED***