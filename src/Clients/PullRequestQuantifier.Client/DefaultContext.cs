namespace PullRequestQuantifier.Client
***REMOVED***
    using System.Collections.Generic;
    using global::PullRequestQuantifier.Abstractions.Context;
    using global::PullRequestQuantifier.Abstractions.Git;

    public sealed class DefaultContext
    ***REMOVED***
        internal static readonly Context Value = new Context
        ***REMOVED***
            LanguageOptions = new LanguageOptions
            ***REMOVED***
                IgnoreCodeBlockSeparator = true,
                IgnoreComments = true,
                IgnoreSpaces = true
    ***REMOVED***,
            DynamicBehaviour = false,
            Thresholds = new List<Threshold>
            ***REMOVED***
                new Threshold
                ***REMOVED***
                    Label = "Extra Small",
                    Value = 9,
                    Color = "Green",
                    GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
        ***REMOVED***,
                new Threshold
                ***REMOVED***
                    Label = "Small",
                    Value = 29,
                    Color = "Green",
                    GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
        ***REMOVED***,
                new Threshold
                ***REMOVED***
                    Label = "Medium",
                    Value = 99,
                    Color = "Yellow",
                    GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
        ***REMOVED***,
                new Threshold
                ***REMOVED***
                    Label = "Large",
                    Value = 499,
                    Color = "Red",
                    GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
        ***REMOVED***,
                new Threshold
                ***REMOVED***
                    Label = "Extra Large",
                    Value = 999,
                    Color = "Red",
                    GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
        ***REMOVED***
    ***REMOVED***,
            Excluded = new List<string> ***REMOVED*** "*.csproj" ***REMOVED***,
            GitOperationType = new List<GitOperationType> ***REMOVED*** GitOperationType.Add, GitOperationType.Delete ***REMOVED***
***REMOVED***;
***REMOVED***
***REMOVED***