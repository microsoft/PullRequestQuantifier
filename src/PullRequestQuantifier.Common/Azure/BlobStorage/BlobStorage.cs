namespace PullRequestQuantifier.Common.Azure.BlobStorage
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <inheritdoc />
    public sealed class BlobStorage : IBlobStorage
    {
        private readonly CloudTableClient cloudTableClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorage" /> class.
        /// </summary>
        /// <param name="azureBlobAccountName">azure blob account name.</param>
        /// <param name="azureBlobAccessKey">azure blob access name.</param>
        /// <param name="useHttps">use https.</param>
        public BlobStorage(string azureBlobAccountName, string azureBlobAccessKey, bool useHttps)
        {
            var cloudStorageAccount =
                new CloudStorageAccount(new StorageCredentials(azureBlobAccountName, azureBlobAccessKey), useHttps);
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorage" /> class.
        /// </summary>
        /// <param name="cloudStorageAccount">A CloudStorageAccount object.</param>
        public BlobStorage(CloudStorageAccount cloudStorageAccount)
        {
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
        }

        /// <inheritdoc />
        public async Task<bool> CreateTableAsync(string tableName)
        {
            // Create the CloudTable object that represents the tableName table.
            CloudTable table = cloudTableClient.GetTableReference(tableName);
            return await table.CreateIfNotExistsAsync();
        }

        /// <inheritdoc />
        public async Task<bool> DeleteTableAsync(string tableName)
        {
            // Create the CloudTable object that represents the tableName table.
            CloudTable table = cloudTableClient.GetTableReference(tableName);
            return await table.DeleteIfExistsAsync();
        }

        /// <inheritdoc />
        public async Task<bool> TableExistsAsync(string tableName)
        {
            // Create the CloudTable object that represents the tableName table.
            CloudTable table = cloudTableClient.GetTableReference(tableName);
            return await table.ExistsAsync();
        }

        /// <inheritdoc />
        public async Task<TableResult> InsertTableEntityAsync(string tableName, TableEntity entity)
        {
            // Create the CloudTable object that represents the tableName table.
            CloudTable table = cloudTableClient.GetTableReference(tableName);

            // Create the TableOperation that inserts the entity.
            TableOperation insertOperation = TableOperation.Insert(entity);

            // Execute the insert operation.
            return await table.ExecuteAsync(insertOperation);
        }

        /// <inheritdoc />
        public async Task<IList<TableResult>> InsertTableEntitiesAsync(string tableName, IEnumerable<TableEntity> entities)
        {
            // Create the CloudTable object that represents the tableName table.
            CloudTable table = cloudTableClient.GetTableReference(tableName);

            // Create the batch operation.
            var batchOperation = new TableBatchOperation();

            foreach (var entity in entities)
            {
                // Add entities to the batch insert operation.
                batchOperation.Insert(entity);
            }

            // Execute the batch operation.
            return await table.ExecuteBatchAsync(batchOperation);
        }

        /// <inheritdoc />
        public async Task<TableResult> InsertOrReplaceTableEntityAsync(string tableName, TableEntity entity)
        {
            // Create the CloudTable object that represents the tableName table.
            CloudTable table = cloudTableClient.GetTableReference(tableName);

            // Create the TableOperation that insert or update the entity.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(entity);

            // Execute the insert operation.
            return await table.ExecuteAsync(insertOrReplaceOperation);
        }

        /// <inheritdoc />
        public async Task<IList<TableResult>> InsertOrReplaceTableEntitiesAsync(string tableName, IEnumerable<TableEntity> entities)
        {
            // Create the CloudTable object that represents the tableName table.
            CloudTable table = cloudTableClient.GetTableReference(tableName);

            // Azure table batch operation has a maximum of 100 operations at a time
            var tableResults = new List<TableResult>();
            var entityList = entities.ToList();
            if (!entityList.Any())
            {
                return tableResults;
            }

            var batchSize = 100;
            for (var page = 0; page < (entityList.Count / batchSize) + 1; page++)
            {
                var entityBatch = entityList.Skip(batchSize * page).Take(batchSize);
                var batchOperation = new TableBatchOperation();

                // Add entities to the batch insert operation.
                foreach (var entity in entityBatch)
                {
                    batchOperation.InsertOrReplace(entity);
                }

                // Execute the batch operation.
                var batchResults = await table.ExecuteBatchAsync(batchOperation);
                tableResults.AddRange(batchResults);
            }

            return tableResults;
        }

        /// <inheritdoc />
        public async Task<TableResult> DeleteTableEntityAsync(string tableName, TableEntity entity)
        {
            // Delete item (the ETAG is required here!).
            entity.ETag = "*";

            // Create the CloudTable object that represents the tableName table.
            CloudTable table = cloudTableClient.GetTableReference(tableName);

            // Create the TableOperation that insert or update the entity.
            TableOperation deleteOperation = TableOperation.Delete(entity);

            // Execute the batch operation.
            return await table.ExecuteAsync(deleteOperation);
        }

        /// <inheritdoc />
        public async Task<T> ReadTableEntityAsync<T>(string tableName, string partitionKey, string rowKey)
            where T : TableEntity, new()
        {
            // Create the CloudTable object that represents the tableName table.
            CloudTable table = cloudTableClient.GetTableReference(tableName);

            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            return retrievedResult.Result != null ? (T)retrievedResult.Result : new T();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> ReadTableEntitiesAsync<T>(
            string tableName,
            string filterCondition)
            where T : TableEntity, new()
        {
            // Create the CloudTable object that represents the tableName table.
            CloudTable table = cloudTableClient.GetTableReference(tableName);

            // Construct the query operation
            TableQuery<T> query = new TableQuery<T>().Where(filterCondition);

            TableContinuationToken token = null;
            var ret = new List<T>();

            do
            {
                TableQuerySegment<T> resultSegment = await table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                ret.AddRange(resultSegment.Results);
            }
            while (token != null);

            return ret;
        }
    }
}
