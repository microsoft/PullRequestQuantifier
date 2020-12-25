namespace PrQuantifier.Client
{
    using System.Threading.Tasks;
    using global::PrQuantifier.Abstractions.Context;

    public interface IContextGenerator
    {
        /// <summary>
        /// Generate a context by given a repo path.
        /// </summary>
        /// <param name="repoPath">The repo  path.</param>
        /// <returns>returns a context.</returns>
        Task<Context> Create(string repoPath);
    }
}
