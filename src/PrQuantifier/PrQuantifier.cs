namespace PrQuantifier
{
    public class PrQuantifier : IPrQuantifier
    {
        public PrQuantifier(string optionsYamlFile)
        {
        }

        public PrQuantifier(QuantifierOptions options)
        {
        }

        public QuantifierResult Quantify()
        {
            throw new System.NotImplementedException();
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
