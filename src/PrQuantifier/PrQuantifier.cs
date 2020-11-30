namespace PrQuantifier
***REMOVED***
    using System.Collections.Generic;
    using global::PrQuantifier.Core;

    public class PrQuantifier : IPrQuantifier
    ***REMOVED***
        private readonly QuantifierOptions options;

        public PrQuantifier(string optionsYamlFile)
        ***REMOVED***
***REMOVED***

        public PrQuantifier(QuantifierOptions options)
        ***REMOVED***
            this.options = options;
***REMOVED***

        /// <inheritdoc />
        public QuantifierResult Quantify(string path)
        ***REMOVED***
            // get git tree changes
            var gitEngine = new GitEngine();
            var gitChangeCounts = gitEngine.GetGitChangeCounts(path);

            // TODO: Categorize changes into a size bucket

            var quantifierResult = new QuantifierResult
            ***REMOVED***
                ChangeCounts = new Dictionary<OperationType, int>
                ***REMOVED***
                    ***REMOVED*** OperationType.Add, gitChangeCounts[GitOperationType.Add] ***REMOVED***,
                    ***REMOVED*** OperationType.Delete, gitChangeCounts[GitOperationType.Delete] ***REMOVED***
        ***REMOVED***
    ***REMOVED***;
            
            return quantifierResult;
***REMOVED***

        public QuantifierResult QuantifyAgainstBranch(string baseBranch)
        ***REMOVED***
            throw new System.NotImplementedException();
***REMOVED***

        public QuantifierResult QuantifyCommit(string commitSha)
        ***REMOVED***
            throw new System.NotImplementedException();
***REMOVED***
***REMOVED***
***REMOVED***
