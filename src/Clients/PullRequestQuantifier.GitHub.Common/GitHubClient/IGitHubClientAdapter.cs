namespace PullRequestQuantifier.GitHub.Common.GitHubClient
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Octokit;

    /// <summary>
    /// Defines the wrapper for the GitHub client.
    /// </summary>
    public interface IGitHubClientAdapter
    {
        GitHubAppSettings GitHubAppSettings { get; }

        /// <summary>
        /// Gets a repository by name.
        /// </summary>
        /// <param name="organizationName">Organization name.</param>
        /// <param name="repositoryName">Repository name.</param>
        /// <returns>Found repository.</returns>
        Task<Repository> GetRepositoryByNameAsync(
            string organizationName,
            string repositoryName);

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
        /// Get raw contents of a file in the given repository.
        /// </summary>
        /// <param name="organizationName">Organization name.</param>
        /// <param name="repositoryName">Repository name.</param>
        /// <param name="path">Pat of the file to get.</param>
        /// <returns>Raw contents of the file.</returns>
        Task<byte[]> GetRawFileAsync(
            string organizationName,
            string repositoryName,
            string path);

        /// <summary>
        /// Get the list of files in pull request.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="pullRequestNumber">Pull Request number.</param>
        /// <returns>List of pull request files.</returns>
        Task<IReadOnlyList<PullRequestFile>> GetPullRequestFilesAsync(
            long repositoryId,
            int pullRequestNumber);

        /// <summary>
        /// Get a pull request.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="pullRequestNumber">Pull Request number.</param>
        /// <returns>Pull request details.</returns>
        Task<PullRequest> GetPullRequestAsync(
            long repositoryId,
            int pullRequestNumber);

        /// <summary>
        /// Get an existing label by name.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="labelName">Label name.</param>
        /// <returns>Label.</returns>
        Task<Label> GetLabelAsync(long repositoryId, string labelName);

        /// <summary>
        /// Update an existing label.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="labelName">Label name.</param>
        /// <param name="labelUpdate">Label update.</param>
        /// <returns>Updated label.</returns>
        Task<Label> UpdateLabelAsync(
            long repositoryId,
            string labelName,
            LabelUpdate labelUpdate);

        /// <summary>
        /// Create a new label in the repository.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="label">Label to create.</param>
        /// <returns>Created label.</returns>
        Task<Label> CreateLabelAsync(long repositoryId, NewLabel label);

        /// <summary>
        /// Get the labels applied to an issue.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="issueNumber">Issue number.</param>
        /// <returns>List of labels on the issue.</returns>
        Task<IReadOnlyList<Label>> GetIssueLabelsAsync(
            long repositoryId,
            int issueNumber);

        /// <summary>
        /// Apply labels to an issue.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="issueNumber">Issue number.</param>
        /// <param name="labels">List of labels to apply.</param>
        /// <returns>Label list.</returns>
        Task<IReadOnlyList<Label>> ApplyLabelAsync(
            long repositoryId,
            int issueNumber,
            string[] labels);

        /// <summary>
        /// Remove a label from an issue.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="issueNumber">Issue number.</param>
        /// <param name="labelName">Label name to remove.</param>
        /// <returns>Removed label.</returns>
        Task<IReadOnlyList<Label>> RemoveLabelFromIssueAsync(
            long repositoryId,
            int issueNumber,
            string labelName);

        /// <summary>
        /// Create a new comment on an issue.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="issueNumber">Issue number.</param>
        /// <param name="comment">The new comment to create.</param>
        /// <returns>Created comment.</returns>
        Task<IssueComment> CreateIssueCommentAsync(
            long repositoryId,
            int issueNumber,
            string comment);

        /// <summary>
        /// Get all comments on an issue.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="issueNumber">Issue number.</param>
        /// <returns>List of comments on the issue.</returns>
        Task<IReadOnlyList<IssueComment>> GetIssueCommentsAsync(
            long repositoryId,
            int issueNumber);

        /// <summary>
        /// Delete a comment.
        /// </summary>
        /// <param name="repositoryId">Repository ID.</param>
        /// <param name="commentId">Comment ID.</param>
        /// <returns>Task.</returns>
        Task DeleteIssueCommentAsync(
            long repositoryId,
            int commentId);
    }
}
