namespace PullRequestQuantifier.Client.QuantifyClient
{
    using System;
    using System.Threading.Tasks;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Core;
    using global::PullRequestQuantifier.Abstractions.Exceptions;
    using global::PullRequestQuantifier.Client.ContextGenerator;
    using global::PullRequestQuantifier.GitEngine;
    using YamlDotNet.Core;

    /// <inheritdoc />
    public sealed class QuantifyClient : IQuantifyClient
    {
        private readonly IPullRequestQuantifier prQuantifier;
        private readonly GitEngine gitEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuantifyClient"/> class.
        /// </summary>
        /// <param name="contextFilePath">The path of the context file.</param>
        public QuantifyClient(
            string contextFilePath)
        {
            var context = LoadContext(contextFilePath);
            prQuantifier = new PullRequestQuantifier(context);
            gitEngine = new GitEngine();
        }

        /// <inheritdoc />
        public Context Context => prQuantifier.Context;

        /// <inheritdoc />
        public async Task<QuantifierResult> Compute(string gitRepoPath)
        {
            // todo add more options and introduce arguments lib QuantifyAgainstBranch, QuantifyCommit
            // get current location changes
            var quantifierInput = GetChanges(gitRepoPath);

            // quantify the changes
            return await prQuantifier.Quantify(quantifierInput);
        }

        /// <inheritdoc />
        public async Task<QuantifierResult> Compute(QuantifierInput quantifierInput)
        {
            // quantify the changes
            return await prQuantifier.Quantify(quantifierInput);
        }

        /// <summary>
        /// this is a comment.
        /// </summary>
        /// <param name="quantifierInput"></param>
        /// <returns></returns>
        public async Task<QuantifierResult> ComputeOverload(QuantifierInput quantifierInput)
        {
            // quantify the changes
            // this is comment
            asdfasddf
                asdfasd
            return await prQuantifier.Quantify(quantifierInput);
        }

        private Context LoadContext(string contextFilePathOrContent)
        {
            var context = DefaultContext.Value;
            if (string.IsNullOrEmpty(contextFilePathOrContent))
            {
                return context;
            }

            // if no valid context will be loaded then load the default one, don't fail
            try
            {
                var ctx = ContextFactory.Load(contextFilePathOrContent);
                context = ctx;
            }
            catch (ThresholdException ex)
            {
                // misconfiguration then print the exception
                Console.WriteLine(ex);
            }
            catch (YamlException ex)
            {
                Console.WriteLine("Yaml could not be parsed");
                Console.WriteLine(ex);
            }
            catch (ArgumentNullException ex)
            {
                // misconfiguration then print the exception
                Console.WriteLine(ex);
            }

            return context;
        }

        private QuantifierInput GetChanges(string repoPath)
        {
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(repoPath));

            return quantifierInput;
        }
    }
}
