namespace PrQuantifier
***REMOVED***
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
        public QuantifierResult Quantify()
        ***REMOVED***
            // get count of current changes

            // return result
            var quantifierResult = new QuantifierResult();
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
