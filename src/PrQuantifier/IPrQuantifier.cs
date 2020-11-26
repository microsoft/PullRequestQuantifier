namespace PrQuantifier
***REMOVED***
    /// <summary>
    /// Assumes invocation from a directory that is part of a
    /// git repository.
    /// </summary>
    public interface IPrQuantifier
    ***REMOVED***
        /// <summary>
        /// Quantifies unstaged changes in current branch.
        /// </summary>
        /// <returns></returns>
        QuantifierResult Quantify();

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
