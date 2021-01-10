namespace PullRequestQuantifier.Abstractions.Git
{
    public enum GitChangeType
    {
        /// <summary>
        /// No changes detected.
        /// </summary>
        Unmodified = 0,

        /// <summary>
        /// The file was added.
        /// </summary>
        Added = 1,

        /// <summary>
        /// The file was deleted.
        /// </summary>
        Deleted = 2,

        /// <summary>
        /// The file content was modified.
        /// </summary>
        Modified = 3,

        /// <summary>
        /// The file was renamed.
        /// </summary>
        Renamed = 4,

        /// <summary>
        /// The file was copied.
        /// </summary>
        Copied = 5,

        /// <summary>
        /// The file is ignored in the work dir.
        /// </summary>
        Ignored = 6,

        /// <summary>
        /// The file is untracked in the work dir.
        /// </summary>
        Untracked = 7,

        /// <summary>
        /// The type (i.e. regular file, symlink, submodule, ...)
        /// of the file was changed.
        /// </summary>
        TypeChanged = 8,

        /// <summary>
        /// Entry is unreadable.
        /// </summary>
        Unreadable = 9,

        /// <summary>
        /// Entry is currently in conflict.
        /// </summary>
        Conflicted = 10
    }
}
