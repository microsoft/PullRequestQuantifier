namespace PullRequestQuantifier.Tools.CloneAdoRepo.Model
{
    using System.Collections.Generic;

    public sealed class Organization
    {
        public string Name { get; set; }

        public IEnumerable<Project> Projects { get; set; }
    }
}
