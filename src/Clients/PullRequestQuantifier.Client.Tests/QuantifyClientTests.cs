﻿namespace PullRequestQuantifier.Client.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using PullRequestQuantifier.Abstractions.Core;
    using PullRequestQuantifier.Client.QuantifyClient;
    using PullRequestQuantifier.Common.Tests;
    using PullRequestQuantifier.GitEngine;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class QuantifyClientTests
    {
        private readonly GitRepoTestHelpers gitRepoHelpers = new GitRepoTestHelpers();
        private readonly GitEngine gitEngine = new GitEngine();

        public QuantifyClientTests()
        {
            gitRepoHelpers.CreateRepo();
        }

        [Fact]
        public async Task QuantifyClient_UsingDefaultContext()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));
            var quantifyClient = new QuantifyClient(string.Empty);

            // Act
            var quantifierResult = await quantifyClient.Compute(quantifierInput);

            // Assert
            Assert.True(!string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(3, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(2, quantifierResult.QuantifiedLinesDeleted);
            Assert.True(Math.Abs(quantifierResult.PercentileAddition - 2) < float.Epsilon);
            Assert.Equal(1, (int)quantifierResult.PercentileDeletion);
            Assert.Equal(3, (int)quantifierResult.FormulaPercentile);
        }

        [Fact]
        public async Task QuantifyClient_MissingFormulaPercentileContext()
        {
            // Arrange
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 2);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 4);
            gitRepoHelpers.CommitFilesToRepo();
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake.cs", 5);
            gitRepoHelpers.AddUntrackedFileToRepoWithNumLines("fake2.cs", 2);
            var quantifierInput = new QuantifierInput();
            quantifierInput.Changes.AddRange(gitEngine.GetGitChanges(gitRepoHelpers.RepoPath));
            var quantifyClient = new QuantifyClient(@"Data/MissingFormulaPercentileContext.prquantifier.yaml");

            // Act
            var quantifierResult = await quantifyClient.Compute(quantifierInput);

            // Assert
            Assert.True(!string.IsNullOrEmpty(quantifierResult.Label));
            Assert.Equal(3, quantifierResult.QuantifiedLinesAdded);
            Assert.Equal(2, quantifierResult.QuantifiedLinesDeleted);
            Assert.Equal(0, quantifierResult.PercentileAddition);
            Assert.Equal(0, quantifierResult.PercentileDeletion);
            Assert.Equal(0, quantifierResult.FormulaPercentile);
        }
    }
}
