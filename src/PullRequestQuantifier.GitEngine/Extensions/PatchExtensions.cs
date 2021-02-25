namespace PullRequestQuantifier.GitEngine.Extensions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using LibGit2Sharp;
    using PullRequestQuantifier.Abstractions.Git;

    public static class PatchExtensions
    {
        public static IEnumerable<GitFilePatch> GetGitFilePatch(this Patch filesPatch, RepositoryStatus status = null)
        {
            var ret = new List<GitFilePatch>();
            using IEnumerator<PatchEntryChanges> patches = filesPatch.GetEnumerator();
            while (patches.MoveNext()
                   && patches.Current != null)
            {
                // minimum 88 score threshold was chose based on observation, if similarity will be less
                // than 88 then the deleted file will be marked as deleted and the other one as addition
                // if the file is part of the rename set the change type explicitly to rename,
                // otherwise git will set it to added or deleted
                ChangeKind changeKind = status?.RenamedInWorkDir.Count(
                    r => r.IndexToWorkDirRenameDetails != null &&
                         (r.IndexToWorkDirRenameDetails.NewFilePath
                              .Equals(patches.Current.Path)
                          || r.IndexToWorkDirRenameDetails
                              .OldFilePath
                              .Equals(patches.Current.Path))
                         && r.IndexToWorkDirRenameDetails
                             .Similarity >= 88) > 0
                    ? ChangeKind.Renamed
                    : patches.Current.Status;

                ret.Add(
                    new GitFilePatch(
                        patches.Current.Path,
                        new FileInfo(patches.Current.Path).Extension)
                    {
                        DiffContent = patches.Current.Patch,
                        AbsoluteLinesAdded = patches.Current.LinesAdded,
                        AbsoluteLinesDeleted = patches.Current.LinesDeleted,
                        ChangeType = changeKind.ConvertToChangeType()
                    });
            }

            return ret;
        }
    }
}
