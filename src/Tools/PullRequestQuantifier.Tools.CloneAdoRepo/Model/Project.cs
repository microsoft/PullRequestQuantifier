namespace PullRequestQuantifier.Tools.CloneAdoRepo.Model
{
    using System.Collections.Generic;

    public sealed class Project
    {
        public string Name { get; set; }

        public IEnumerable<Repository> Repositories { get; set; }
    }
}
