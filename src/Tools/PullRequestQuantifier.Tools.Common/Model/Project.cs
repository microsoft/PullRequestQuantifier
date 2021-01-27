namespace PullRequestQuantifier.Tools.Common.Model
{
    using System.Collections.Generic;

    public sealed class Project
    {
        public string Name { get; set; }

        public IEnumerable<Repository> Repositories { get; set; }
    }
}
