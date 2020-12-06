namespace PrQuantifier.Core.Model
{
    using System.Collections.Generic;

    public sealed class GitPr
    {
        public IDictionary<GitOperationType,int> FilesPatch { get; set; }
    }
}
