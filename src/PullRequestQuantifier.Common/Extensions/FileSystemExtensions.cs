namespace PullRequestQuantifier.Common.Extensions
{
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    public static class FileSystemExtensions
    {
        public static void DeleteDirectory(this IFileSystem fileSystem, string path)
        {
#pragma warning disable CS0618 // Do not catch general exception types
            var dirInfo = fileSystem.DirectoryInfo.New(path);
#pragma warning restore CS0618 // Do not catch general exception types

            if (dirInfo.Exists)
            {
                try
                {
                    SetNormalAttribute(dirInfo);
                    dirInfo.Delete(true);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
#pragma warning restore CA1031 // Do not catch general exception types
                {
                }
            }
        }

        private static void SetNormalAttribute(IDirectoryInfo dirInfo)
        {
            if (!dirInfo.Exists)
            {
                return;
            }

            var directories = dirInfo.GetDirectories().ToArray();
            foreach (var subDir in directories)
            {
                SetNormalAttribute(subDir);
            }

            var files = dirInfo.GetFiles().ToArray();
            foreach (var file in files)
            {
                file.Attributes = FileAttributes.Normal;
            }
        }
    }
}
