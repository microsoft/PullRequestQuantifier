namespace PrQuantifier
***REMOVED***
    public interface IPrQuantifier
    ***REMOVED***
        /// <summary>
        /// Quantifies unstaged changes in current branch.
        /// </summary>
        /// <param name="path">Path to any item in the repository.</param>
        /// <returns>QuantifierResult.</returns>
        QuantifierResult Quantify(string path);

        /// <summary>
        /// Quantifies current branch against a base branch.
        /// Includes all missing commits and unstaged changes.
        /// </summary>
        /// <param name="baseBranch">Branch to quantify against (eg. refs/heads/main).</param>
        /// <returns></returns>
        QuantifierResult QuantifyAgainstBranch(string baseBranch);

        /// <summary>
        /// Quantifies a given commit in the current branch.
        /// </summary>
        /// <param name="commitSha">Commit SHA.</param>
        /// <returns></returns>
        QuantifierResult QuantifyCommit(string commitSha);
***REMOVED***
***REMOVED***
