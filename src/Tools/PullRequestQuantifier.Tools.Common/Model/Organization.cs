namespace PullRequestQuantifier.Tools.Common.Model
{
    using System.Collections.Generic;

    public sealed class Organization
    {
        public string Name { get; set; }

        public IEnumerable<Project> Projects { get; set; }
    }
}
