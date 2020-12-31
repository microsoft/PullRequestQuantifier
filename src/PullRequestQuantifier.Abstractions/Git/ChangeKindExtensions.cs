namespace PullRequestQuantifier.Abstractions.Git
***REMOVED***
    using System;
    using LibGit2Sharp;

    public static class ChangeKindExtensions
    ***REMOVED***
        public static GitChangeType ConvertToChangeType(this ChangeKind changeKind)
        ***REMOVED***
            return changeKind switch
            ***REMOVED***
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
    ***REMOVED***;
***REMOVED***
***REMOVED***
***REMOVED***
