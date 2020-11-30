namespace PrQuantifier
{
    using System.Collections.Generic;
    using global::PrQuantifier.Core;

    public class PrQuantifier : IPrQuantifier
    {
        private readonly QuantifierOptions options;

        public PrQuantifier(string optionsYamlFile)
        {
        }

        public PrQuantifier(QuantifierOptions options)
        {
            this.options = options;
        }

        /// <inheritdoc />
        public QuantifierResult Quantify(string path)
        {
            // get git tree changes
            var gitEngine = new GitEngine();
            var gitChangeCounts = gitEngine.GetGitChangeCounts(path);

            // TODO: Categorize changes into a size bucket

            var quantifierResult = new QuantifierResult
            {
                ChangeCounts = new Dictionary<OperationType, int>
                {
                    { OperationType.Add, gitChangeCounts[GitOperationType.Add] },
                    { OperationType.Delete, gitChangeCounts[GitOperationType.Delete] }
                }
            };
            
            return quantifierResult;
        }

        public QuantifierResult QuantifyAgainstBranch(string baseBranch)
        {
            throw new System.NotImplementedException();
        }

        public QuantifierResult QuantifyCommit(string commitSha)
        {
            throw new System.NotImplementedException();
        }
    }
}
