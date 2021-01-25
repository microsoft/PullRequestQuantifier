namespace PullRequestQuantifier.GitEngine.Tests.Helpers.Diff
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using PullRequestQuantifier.GitEngine.Helpers.Diff;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class GitCreateDiffTests
    {
        [Fact]
        public void CreateDiff_Test()
        {
            // Setup
            using var gitDiff = new GitCreateDiff();

            // Act
            var change = gitDiff.CreateDiff(
                File.ReadAllText(@"Data/0CA446AAB9D09EAC8625B53E3DF8DA661976C458.md"),
                File.ReadAllText(@"Data/E6AFB500B25D1CE9A145EFFE40A0206F2B40C9F8.md"));

            // Assert
            Assert.NotNull(change);
            Assert.Equal(1, change.AbsoluteLinesAdded);
            Assert.Equal(1, change.AbsoluteLinesDeleted);
            Assert.True(change.DiffContent.IndexOf("-# Introduction", StringComparison.Ordinal) > -1);
            Assert.True(change.DiffContent.IndexOf("+# Introduction - test PG", StringComparison.Ordinal) > -1);
        }
    }
}
