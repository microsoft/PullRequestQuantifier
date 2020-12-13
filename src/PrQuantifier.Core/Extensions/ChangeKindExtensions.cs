namespace PrQuantifier.Core.Extensions
{
    using System;
    using LibGit2Sharp;
    using PrQuantifier.Core.Git;

    public static class ChangeKindExtensions
    {
        public static GitChangeType ConvertToChangeType(this ChangeKind changeKind)
        {
            switch (changeKind)
            {
                case ChangeKind.Unmodified:
                    return GitChangeType.Unmodified;
                case ChangeKind.Added:
                    return GitChangeType.Added;
                case ChangeKind.Deleted:
                    return GitChangeType.Deleted;
                case ChangeKind.Modified:
                    return GitChangeType.Modified;
                case ChangeKind.Renamed:
                    return GitChangeType.Renamed;
                case ChangeKind.Copied:
                    return GitChangeType.Copied;
                case ChangeKind.Ignored:
                    return GitChangeType.Ignored;
                case ChangeKind.Untracked:
                    return GitChangeType.Untracked;
                case ChangeKind.TypeChanged:
                    return GitChangeType.TypeChanged;
                case ChangeKind.Unreadable:
                    return GitChangeType.Unreadable;
                case ChangeKind.Conflicted:
                    return GitChangeType.Conflicted;
                default:
                    throw new ArgumentOutOfRangeException(nameof(changeKind), changeKind, null);
            }
        }
    }
}
