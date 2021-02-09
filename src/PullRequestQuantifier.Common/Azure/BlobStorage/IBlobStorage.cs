namespace PullRequestQuantifier.Common.Azure.BlobStorage
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Azure blob storage wrapper.
    /// </summary>
    public interface IBlobStorage
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>True if the table was created successfully.</returns>
        Task<bool> CreateTableAsync(string tableName);

        /// <summary>
        /// Deletes the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if success, otherwise false.</returns>
        Task<bool> DeleteTableAsync(string tableName);

        /// <summary>
        /// Tables the exists.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if success, otherwise false.</returns>
        Task<bool> TableExistsAsync(string tableName);

        /// <summary>
        /// Inserts the table entity.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>returns the new inserted result.</returns>
        Task<TableResult> InsertTableEntityAsync(string tableName, TableEntity entity);

        /// <summary>
        /// Inserts the table entities.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entities">The entities.</param>
        /// <returns>returns the new inserted result.</returns>
        Task<IList<TableResult>> InsertTableEntitiesAsync(string tableName, IEnumerable<TableEntity> entities);

        /// <summary>
        /// Inserts the or replace table entity.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>returns the new inserted or updated result.</returns>
        Task<TableResult> InsertOrReplaceTableEntityAsync(string tableName, TableEntity entity);

        /// <summary>
        /// Inserts the or replace table entities.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entities">The entities.</param>
        /// <returns>returns the new inserted or updated results.</returns>
        Task<IList<TableResult>> InsertOrReplaceTableEntitiesAsync(string tableName, IEnumerable<TableEntity> entities);

        /// <summary>
        /// Deletes the table entity.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>returns deleted result.</returns>
        Task<TableResult> DeleteTableEntityAsync(string tableName, TableEntity entity);

        /// <summary>
        /// Read table entity.
        /// </summary>
        /// <typeparam name="T">Entity object to return.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <returns>TableResult instance.</returns>
        Task<T> ReadTableEntityAsync<T>(string tableName, string partitionKey, string rowKey)
            where T : TableEntity, new();

        /// <summary>
        /// Read table entities.
        /// </summary>
        /// <typeparam name="T">Entity object to return.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="filterCondition">The filtering condition ( use TableQuery.GenerateFilterCondition to construct it).</param>
        /// <returns>TableResult instance.</returns>
        Task<IEnumerable<T>> ReadTableEntitiesAsync<T>(string tableName, string filterCondition)
            where T : TableEntity, new();
    }
}
