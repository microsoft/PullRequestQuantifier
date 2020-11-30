namespace PrQuantifier
{
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
        public QuantifierResult Quantify()
        {
            // get count of current changes

            // return result
            var quantifierResult = new QuantifierResult();
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
