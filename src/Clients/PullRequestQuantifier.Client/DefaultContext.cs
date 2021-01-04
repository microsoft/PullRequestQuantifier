namespace PullRequestQuantifier.Client
{
    using System.Collections.Generic;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Git;

    public sealed class DefaultContext
    {
        internal static readonly Context Value = new Context
        {
            LanguageOptions = new LanguageOptions
            {
                IgnoreCodeBlockSeparator = true,
                IgnoreComments = true,
                IgnoreSpaces = true
            },
            DynamicBehaviour = false,
            Thresholds = new List<Threshold>
            {
                new Threshold
                {
                    Label = "Extra Small",
                    Value = 9,
                    Color = "Green",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                },
                new Threshold
                {
                    Label = "Small",
                    Value = 29,
                    Color = "Green",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                },
                new Threshold
                {
                    Label = "Medium",
                    Value = 99,
                    Color = "Yellow",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                },
                new Threshold
                {
                    Label = "Large",
                    Value = 499,
                    Color = "Red",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                },
                new Threshold
                {
                    Label = "Extra Large",
                    Value = 999,
                    Color = "Red",
                    GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
                }
            },
            Excluded = new List<string> { "*.csproj" },
            GitOperationType = new List<GitOperationType> { GitOperationType.Add, GitOperationType.Delete }
        };
    }
}