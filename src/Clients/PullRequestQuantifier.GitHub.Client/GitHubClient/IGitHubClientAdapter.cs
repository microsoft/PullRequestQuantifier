namespace PullRequestQuantifier.GitHub.Client.GitHubClient
***REMOVED***
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Octokit;

    /// <summary>
    /// Defines the wrapper for the GitHub client.
    /// </summary>
    public interface IGitHubClientAdapter
    ***REMOVED***
        /// <summary>
        /// Gets a repository by name.
        /// </summary>
        /// <param name="organizationName">Organization name.</param>
        /// <param name="repositoryName">Repository name.</param>
        /// <returns>Found repository.</returns>
        Task<Repository> GetRepositoryByNameAsync(string organizationName, string repositoryName);

        /// <summary>
        /// Gets the specified repository.
        /// </summary>
        /// <param name="repositoryId">The Id of the repository.</param>
        /// <returns>returns a repository.</returns>
        Task<Repository> GetRepositoryByIdAsync(long repositoryId);

        /// <summary>
        /// Gets all repositories for an organization.
        /// </summary>
        /// <param name="organizationName">Organization name.</param>
        /// <returns>returns a collection of repos.</returns>
        Task<IReadOnlyList<Repository>> GetRepositoriesForOrganizationAsync(string organizationName);

        /// <summary>
        /// Gets all commits for a repository and given path.
        /// </summary>
        /// <param name="organizationName">Organization name.</param>
        /// <param name="repositoryName">Repository name.</param>
        /// <param name="path">Path to file.</param>
        /// <returns>List of commits to given repository and path.</returns>
        Task<IReadOnlyList<GitHubCommit>> GetCommitsAsync(
            string organizationName,
            string repositoryName,
            string path = "");

        /// <summary>
        /// Gets contents of a file or directory by ref in a given repository.
        /// </summary>
        /// <param name="organizationName">Organization name.</param>
        /// <param name="repositoryName">Repository name.</param>
        /// <param name="reference">Git reference.</param>
        /// <param name="path">Path to file.</param>
        /// <returns>Contents of the given path.</returns>
        Task<IReadOnlyList<RepositoryContent>> GetRepositoryContentByRefAsync(
            string organizationName,
            string repositoryName,
            string reference,
            string path = "");

        /// <summary>
        /// Get a git tree recursively for a given reference.
        /// </summary>
        /// <param name="organizationName">Organization name.</param>
        /// <param name="repositoryName">Repository name.</param>
        /// <param name="reference">Git reference.</param>
        /// <returns>Git tree object.</returns>
        Task<TreeResponse> GetGitTreeRecursiveAsync(
            string organizationName,
            string repositoryName,
            string reference);

        /// <summary>
        /// Get a git blob by reference.
        /// </summary>
        /// <param name="organizationName">Organization name.</param>
        /// <param name="repositoryName">Repository name.</param>
        /// <param name="reference">Git reference.</param>
        /// <returns>Git blob.</returns>
        Task<Blob> GetGitBlobAsync(
            string organizationName,
            string repositoryName,
            string reference);

        /// <summary>
        /// Creates a commit that creates a new file in the repository.
        /// </summary>
        /// <param name="organizationName">Organization name.</param>
        /// <param name="repositoryName">Repository name.</param>
        /// <param name="path">Path of the file to create.</param>
        /// <param name="createFileRequest">Details of the file to be created.</param>
        /// <returns>Content change set.</returns>
        Task<RepositoryContentChangeSet> CreateFileAsync(
            string organizationName,
            string repositoryName,
            string path,
            CreateFileRequest createFileRequest);

        /// <summary>
        /// Creates a commit that deletes a file in the repository.
        /// </summary>
        /// <param name="organizationName">Organization name.</param>
        /// <param name="repositoryName">Repository name.</param>
        /// <param name="path">Path of the file to create.</param>
        /// <param name="deleteFileRequest">Details of the file to be deleted.</param>
        /// <returns>Content change set.</returns>
        Task DeleteFileAsync(
            string organizationName,
            string repositoryName,
            string path,
            DeleteFileRequest deleteFileRequest);

        /// <summary>
        /// Get the list of files in pull request.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="pullRequestNumber">Pull Reqeust number.</param>
        /// <returns>List of pull request files.</returns>
        Task<IReadOnlyList<PullRequestFile>> GetPullRequestFiles(long repositoryId, int pullRequestNumber);
***REMOVED***
***REMOVED***