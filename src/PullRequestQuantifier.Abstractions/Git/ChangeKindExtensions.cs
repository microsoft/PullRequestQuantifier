namespace PullRequestQuantifier.Abstractions.Git
{
    using System;
    using LibGit2Sharp;

    public static class ChangeKindExtensions
    {
        public static GitChangeType ConvertToChangeType(this ChangeKind changeKind)
        {
            return changeKind switch
            {
                ChangeKind.Unmodified => GitChangeType.Unmodified,
                ChangeKind.Added => GitChangeType.Added,
                ChangeKind.Deleted => GitChangeType.Deleted,
                ChangeKind.Modified => GitChangeType.Modified,
                ChangeKind.Renamed => GitChangeType.Renamed,
                ChangeKind.Copied => GitChangeType.Copied,
                ChangeKind.Ignored => GitChangeType.Ignored,
                ChangeKind.Untracked => GitChangeType.Untracked,
                ChangeKind.TypeChanged => GitChangeType.TypeChanged,
                ChangeKind.Unreadable => GitChangeType.Unreadable,
                ChangeKind.Conflicted => GitChangeType.Conflicted,
                _ => throw new ArgumentOutOfRangeException(nameof(changeKind), changeKind, null)
            };
        }
    }
}
