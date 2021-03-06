namespace PullRequestQuantifier.Common.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Abstractions;
    using PullRequestQuantifier.Common.Extensions;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public sealed class FileSystemExtensionsTests
    {
        [Fact]
        public void FileSystemExtensions()
        {
            // setup
            var fileSystem = new FileSystem();
            var tempPath = Path.Combine(
                fileSystem.Path.GetTempPath(),
                Guid.NewGuid().ToString());
            fileSystem.Directory.CreateDirectory(tempPath);

            // act
            fileSystem.DeleteDirectory(tempPath);

            // assert
            Assert.False(Directory.Exists(tempPath));
        }
    }
}
