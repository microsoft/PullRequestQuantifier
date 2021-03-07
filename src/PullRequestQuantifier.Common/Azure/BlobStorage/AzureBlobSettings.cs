namespace PullRequestQuantifier.Common.Azure.BlobStorage
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public sealed class AzureBlobSettings
    {
        public string AccountName { get; set; }

        public string AccountKey { get; set; }
    }
}
