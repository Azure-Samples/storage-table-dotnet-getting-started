//----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//----------------------------------------------------------------------------------

namespace TableStorage
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Model;

    /// <summary>
    /// Azure Table Service Sample - Demonstrate how to perform common tasks using the Microsoft Azure Table storage 
    /// including creating a table, CRUD operations, batch operations and different querying techniques. 
    /// 
    /// Note: This sample uses the .NET 4.5 asynchronous programming model to demonstrate how to call the Storage Service using the 
    /// storage client libraries asynchronous API's. When used in real applications this approach enables you to improve the 
    /// responsiveness of your application. Calls to the storage service are prefixed by the await keyword. 
    /// 
    /// Documentation References: 
    /// - How to create, manage, or delete a storage account in the Azure Portal - https://azure.microsoft.com/documentation/articles/storage-create-storage-account/
    /// - Getting Started with Tables - http://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-tables/
    /// - Table Service Concepts - http://msdn.microsoft.com/en-us/library/dd179463.aspx
    /// - Table Service REST API - http://msdn.microsoft.com/en-us/library/dd179423.aspx
    /// - Table Service C# API - http://go.microsoft.com/fwlink/?LinkID=398944
    /// - Use the Azure Storage Emulator for Development and Testing - https://azure.microsoft.com/documentation/articles/storage-use-emulator/
    /// - Asynchronous Programming with Async and Await  - http://msdn.microsoft.com/en-us/library/hh191443.aspx
    /// </summary>
    public class Program
    {
        // *************************************************************************************************************************
        // Instructions: This sample can be run using either the Azure Storage Emulator that installs as part of this SDK - or by
        // updating the App.Config file with your AccountName and Key. 
        // 
        // To run the sample using the Storage Emulator (default option)
        //      1. Start the Azure Storage Emulator (once only) by pressing the Start button or the Windows key and searching for it
        //         by typing "Azure Storage Emulator". Select it from the list of applications to start it.
        //      2. Set breakpoints and run the project using F10. 
        // 
        // To run the sample using the Storage Service
        //      1. Open the app.config file and comment out the connection string for the emulator (UseDevelopmentStorage=True) and
        //         uncomment the connection string for the storage service (AccountName=[]...)
        //      2. Create a Storage Account through the Azure Portal and provide your [AccountName] and [AccountKey] in 
        //         the App.Config file. See http://go.microsoft.com/fwlink/?LinkId=325277 for more information
        //      3. Set breakpoints and run the project using F10. 
        // 
        // *************************************************************************************************************************

        /// <summary>
        /// The name of the table to create for the sample.
        /// </summary>
        internal const string TableName = "customer";

        public static void Main(string[] args)
        {
            Console.WriteLine("Azure Table Storage - Getting Started Samples\n");

            // Create or reference an existing table
            CloudTable table = CreateTableAsync().Result;
            CloudTableClient tableClient = table.ServiceClient;

            // Demonstrate basic CRUD functionality 
            BasicDataOperationsAsync(table).Wait();

            // Demonstrate advanced functionality such as batch operations and segmented multi-entity queries
            AdvancedDataOperationsAsync(table).Wait();

            // List tables in the storage account.
            TableListingOperations(tableClient).Wait();

            // Create a SAS and try CRUD operations with the SAS.
            BasicDataOperationsWithSasAsync(table).Wait();

            // When you delete a table it could take several seconds before you can recreate a table with the same
            // name. The sample table is not deleted by default, so that you can run the demo in quick succession.
            // To delete the table, uncomment the line of code below. 
            // table.DeleteIfExistsAsync().Wait();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.Read();
        }

        /// <summary>
        /// Create a table for the sample application to process messages in. 
        /// </summary>
        /// <returns>A CloudTable object</returns>
        private static async Task<CloudTable> CreateTableAsync()
        {
            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            Console.WriteLine("1. Create a Table for the demo");

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(TableName);
            try
            {
                if (await table.CreateIfNotExistsAsync())
                {
                    Console.WriteLine("Created Table named: {0}", TableName);
                }
                else
                {
                    Console.WriteLine("Table {0} already exists", TableName);
                }
            }
            catch (StorageException)
            {
                Console.WriteLine("If you are running with the default configuration please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            Console.WriteLine();
            return table;
        }

        /// <summary>
        /// Demonstrate basic Table CRUD operations.
        /// </summary>
        /// <param name="table">The sample table</param>
        /// <returns>A Task object</returns>
        private static async Task BasicDataOperationsAsync(CloudTable table)
        {
            // Create an instance of a customer entity. See the Model\CustomerEntity.cs for a description of the entity.
            CustomerEntity customer = new CustomerEntity("Harp", "Walter")
            {
                Email = "Walter@contoso.com",
                PhoneNumber = "425-555-0101"
            };

            // Demonstrate how to Update the entity by changing the phone number
            Console.WriteLine("2. Update an existing Entity using the InsertOrMerge Upsert Operation.");
            customer.PhoneNumber = "425-555-0105";
            customer = await InsertOrMergeEntityAsync(table, customer);
            Console.WriteLine();

            // Demonstrate how to Read the updated entity using a point query 
            Console.WriteLine("3. Reading the updated Entity.");
            customer = await RetrieveEntityUsingPointQueryAsync(table, "Harp", "Walter");
            Console.WriteLine();

            // Demonstrate how to Delete an entity
            Console.WriteLine("4. Delete the entity. ");
            await DeleteEntityAsync(table, customer);
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrate advanced table functionality including batch operations and segmented queries
        /// </summary>
        /// <param name="table">The sample table</param>
        /// <returns>A Task object</returns>
        private static async Task AdvancedDataOperationsAsync(CloudTable table)
        {
            // Demonstrate upsert and batch table operations
            Console.WriteLine("4. Inserting a batch of entities. ");
            await BatchInsertOfCustomerEntitiesAsync(table);
            Console.WriteLine();

            // Query a range of data within a partition using a simple query
            Console.WriteLine("5. Retrieving entities with surname of Smith and first names >= 1 and <= 75");
            ExecuteSimpleQuery(table, "Smith", "0001", "0075");
            Console.WriteLine();

            // Query the same range of data within a partition and return result segments of 50 entities at a time
            Console.WriteLine("6. Retrieving entities with surname of Smith and first names >= 1 and <= 75");
            await PartitionRangeQueryAsync(table, "Smith", "0001", "0075");
            Console.WriteLine();

            // Query for all the data within a partition 
            Console.WriteLine("7. Retrieve entities with surname of Smith.");
            await PartitionScanAsync(table, "Smith");
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates basic CRUD operations using a SAS for authentication.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>A Task object</returns>
        private static async Task BasicDataOperationsWithSasAsync(CloudTable table)
        {
            string sharedAccessPolicyName = "sample-policy-" + DateTime.Now.Ticks.ToString();

            // Create a shared access policy on the table.
            // The access policy may be optionally used to provide constraints for
            // shared access signatures on the table.
            await CreateSharedAccessPolicy(table, sharedAccessPolicyName);

            // Generate an ad-hoc SAS on the table, then test the SAS. It permits all CRUD operations on the table.
            string adHocTableSAS = GetTableSasUri(table);

            // Create an instance of a customer entity.
            CustomerEntity customer1 = new CustomerEntity("Johnson", "Mary")
            {
                Email = "mary@contoso.com",
                PhoneNumber = "425-555-0105"
            };
            await TestTableSAS(adHocTableSAS, customer1);

            // Generate a SAS URI for the table, using the stored access policy to set constraints on the SAS.
            // Then test the SAS. All CRUD operations should succeed.
            string sharedPolicyTableSAS = GetTableSasUri(table, sharedAccessPolicyName);
            
            // Create an instance of a customer entity.
            CustomerEntity customer2 = new CustomerEntity("Wilson", "Joe")
            {
                Email = "joe@contoso.com",
                PhoneNumber = "425-555-0106"
            };
            await TestTableSAS(sharedPolicyTableSAS, customer2);
        }

        /// <summary>
        /// List tables in the storage account.
        /// </summary>
        /// <param name="tableClient">The table client.</param>
        /// <returns>A Task object</returns>
        private static async Task TableListingOperations(CloudTableClient tableClient)
        {
            try
            {
                // To list all tables in the storage account, uncomment the following line.
                // Note that listing all tables in the account may take a long time if the account contains a large number of tables.
                // ListAllTables(tableClient);

                // List tables beginning with the specified prefix.
                await ListTablesWithPrefix(tableClient, "c");

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Validate the connection string information in app.config and throws an exception if it looks like 
        /// the user hasn't updated this to valid values. 
        /// </summary>
        /// <param name="storageConnectionString">Connection string for the storage service or the emulator</param>
        /// <returns>CloudStorageAccount object</returns>
        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

        /// <summary>
        /// The Table Service supports two main types of insert operations. 
        ///  1. Insert - insert a new entity. If an entity already exists with the same PK + RK an exception will be thrown.
        ///  2. Replace - replace an existing entity. Replace an existing entity with a new entity. 
        ///  3. Insert or Replace - insert the entity if the entity does not exist, or if the entity exists, replace the existing one.
        ///  4. Insert or Merge - insert the entity if the entity does not exist or, if the entity exists, merges the provided entity properties with the already existing ones.
        /// </summary>
        /// <param name="table">The sample table name</param>
        /// <param name="entity">The entity to insert or merge</param>
        /// <returns>A Task object</returns>
        private static async Task<CustomerEntity> InsertOrMergeEntityAsync(CloudTable table, CustomerEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                CustomerEntity insertedCustomer = result.Result as CustomerEntity;

                return insertedCustomer;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Demonstrate the most efficient storage query - the point query - where both partition key and row key are specified. 
        /// </summary>
        /// <param name="table">Sample table name</param>
        /// <param name="partitionKey">Partition key - i.e., last name</param>
        /// <param name="rowKey">Row key - i.e., first name</param>
        /// <returns>A Task object</returns>
        private static async Task<CustomerEntity> RetrieveEntityUsingPointQueryAsync(CloudTable table, string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                CustomerEntity customer = result.Result as CustomerEntity;
                if (customer != null)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", customer.PartitionKey, customer.RowKey, customer.Email, customer.PhoneNumber);
                }

                return customer;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="table">Sample table name</param>
        /// <param name="deleteEntity">Entity to delete</param>
        /// <returns>A Task object</returns>
        private static async Task DeleteEntityAsync(CloudTable table, CustomerEntity deleteEntity)
        {
            try
            {
                if (deleteEntity == null)
                {
                    throw new ArgumentNullException("deleteEntity");
                }

                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                await table.ExecuteAsync(deleteOperation);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Demonstrate inserting of a large batch of entities. Some considerations for batch operations:
        ///  1. You can perform updates, deletes, and inserts in the same single batch operation.
        ///  2. A single batch operation can include up to 100 entities.
        ///  3. All entities in a single batch operation must have the same partition key.
        ///  4. While it is possible to perform a query as a batch operation, it must be the only operation in the batch.
        ///  5. Batch size must be less than or equal to 4 MB
        /// </summary>
        /// <param name="table">Sample table name</param>
        /// <returns>A Task object</returns>
        private static async Task BatchInsertOfCustomerEntitiesAsync(CloudTable table)
        {
            try
            {
                // Create the batch operation. 
                TableBatchOperation batchOperation = new TableBatchOperation();

                // The following code  generates test data for use during the query samples.  
                for (int i = 0; i < 100; i++)
                {
                    batchOperation.InsertOrMerge(new CustomerEntity("Smith", string.Format("{0}", i.ToString("D4")))
                    {
                        Email = string.Format("{0}@contoso.com", i.ToString("D4")),
                        PhoneNumber = string.Format("425-555-{0}", i.ToString("D4"))
                    });
                }

                // Execute the batch operation.
                IList<TableResult> results = await table.ExecuteBatchAsync(batchOperation);
                foreach (var res in results)
                {
                    var customerInserted = res.Result as CustomerEntity;
                    Console.WriteLine("Inserted entity with\t Etag = {0} and PartitionKey = {1}, RowKey = {2}", customerInserted.ETag, customerInserted.PartitionKey, customerInserted.RowKey);
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Demonstrate a partition range query that searches within a partition for a set of entities that are within a 
        /// specific range. This query returns all entities in the range. Note that if your table contains a large amount of data,
        /// the query may be slow or may time out. In that case, use a segmented query, as shown in the PartitionRangeQueryAsync() 
        /// sample method.
        /// Note that the ExecuteSimpleQuery method is called synchronously, for the purposes of the sample. However, in a real-world
        /// application using the async/await pattern, best practices recommend using asynchronous methods consistently.
        /// </summary>
        /// <param name="table">Sample table name</param>
        /// <param name="partitionKey">The partition within which to search</param>
        /// <param name="startRowKey">The lowest bound of the row key range within which to search</param>
        /// <param name="endRowKey">The highest bound of the row key range within which to search</param>
        private static void ExecuteSimpleQuery(CloudTable table, string partitionKey, string startRowKey, string endRowKey)
        {
            try
            {
                // Create the range query using the fluid API 
                TableQuery<CustomerEntity> rangeQuery = new TableQuery<CustomerEntity>().Where(
                    TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                            TableOperators.And,
                            TableQuery.CombineFilters(
                                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, startRowKey),
                                TableOperators.And,
                                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, endRowKey))));

                foreach (CustomerEntity entity in table.ExecuteQuery(rangeQuery))
                {
                    Console.WriteLine("Customer: {0},{1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey, entity.Email, entity.PhoneNumber);
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Demonstrate a partition range query that searches within a partition for a set of entities that are within a 
        /// specific range. The async APIs require that the user handle the segment size and return the next segment 
        /// using continuation tokens. 
        /// </summary>
        /// <param name="table">Sample table name</param>
        /// <param name="partitionKey">The partition within which to search</param>
        /// <param name="startRowKey">The lowest bound of the row key range within which to search</param>
        /// <param name="endRowKey">The highest bound of the row key range within which to search</param>
        /// <returns>A Task object</returns>
        private static async Task PartitionRangeQueryAsync(CloudTable table, string partitionKey, string startRowKey, string endRowKey)
        {
            try
            {
                // Create the range query using the fluid API 
                TableQuery<CustomerEntity> rangeQuery = new TableQuery<CustomerEntity>().Where(
                    TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                            TableOperators.And,
                            TableQuery.CombineFilters(
                                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, startRowKey),
                                TableOperators.And,
                                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, endRowKey))));

                // Request 50 results at a time from the server. 
                TableContinuationToken token = null;
                rangeQuery.TakeCount = 50;
                int segmentNumber = 0;
                do
                {
                    // Execute the query, passing in the continuation token.
                    // The first time this method is called, the continuation token is null. If there are more results, the call
                    // populates the continuation token for use in the next call.
                    TableQuerySegment<CustomerEntity> segment = await table.ExecuteQuerySegmentedAsync(rangeQuery, token);

                    // Indicate which segment is being displayed
                    if (segment.Results.Count > 0)
                    {
                        segmentNumber++;
                        Console.WriteLine();
                        Console.WriteLine("Segment {0}", segmentNumber);
                    }

                    // Save the continuation token for the next call to ExecuteQuerySegmentedAsync
                    token = segment.ContinuationToken;

                    // Write out the properties for each entity returned.
                    foreach (CustomerEntity entity in segment)
                    {
                        Console.WriteLine("\t Customer: {0},{1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey, entity.Email, entity.PhoneNumber);
                    }

                    Console.WriteLine();
                }
                while (token != null);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Demonstrate a partition scan whereby we are searching for all the entities within a partition. Note this is not as efficient 
        /// as a range scan - but definitely more efficient than a full table scan. The async APIs require that the user handle the segment 
        /// size and return the next segment using continuation tokens.
        /// </summary>
        /// <param name="table">Sample table name</param>
        /// <param name="partitionKey">The partition within which to search</param>
        /// <returns>A Task object</returns>
        private static async Task PartitionScanAsync(CloudTable table, string partitionKey)
        {
            try
            {
                TableQuery<CustomerEntity> partitionScanQuery =
            new TableQuery<CustomerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                TableContinuationToken token = null;

                // Read entities from each query segment.
                do
                {
                    TableQuerySegment<CustomerEntity> segment = await table.ExecuteQuerySegmentedAsync(partitionScanQuery, token);
                    token = segment.ContinuationToken;
                    foreach (CustomerEntity entity in segment)
                    {
                        Console.WriteLine("Customer: {0},{1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey, entity.Email, entity.PhoneNumber);
                    }
                }
                while (token != null);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Lists all tables in the storage account.
        /// </summary>
        /// <param name="tableClient">The Table storage service client object.</param>
        private static void ListAllTables(CloudTableClient tableClient)
        {
            Console.WriteLine("List all tables in account:");

            try
            {
                // Note that listing all tables in the account may take a long time if the account contains a large number of tables.
                foreach (var table in tableClient.ListTables())
                {
                    Console.WriteLine("\tTable:" + table.Name);
                }

                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Lists tables in the storage account whose names begin with the specified prefix.
        /// </summary>
        /// <param name="tableClient">The Table service client object.</param>
        /// <param name="prefix">The table name prefix.</param>
        /// <returns>A Task object</returns>
        private static async Task ListTablesWithPrefix(CloudTableClient tableClient, string prefix)
        {
            Console.WriteLine("List all tables beginning with prefix {0}:", prefix);

            TableContinuationToken continuationToken = null;
            TableResultSegment resultSegment = null;

            try
            {
                do
                {
                    // List tables beginning with the specified prefix. 
                    // Passing in null for the maxResults parameter returns the maximum number of results (up to 5000).
                    resultSegment = await tableClient.ListTablesSegmentedAsync(
                        prefix, null, continuationToken, null, null);

                    // Enumerate the tables returned.
                    foreach (var table in resultSegment.Results)
                    {
                        Console.WriteLine("\tTable:" + table.Name);
                    }
                }
                while (continuationToken != null);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Creates a shared access policy on the table.
        /// </summary>
        /// <param name="table">A CloudTable object.</param>
        /// <param name="policyName">The name of the stored access policy.</param>
        /// <returns>A Task object</returns>
        private static async Task CreateSharedAccessPolicy(CloudTable table, string policyName)
        {
            // Create a new shared access policy and define its constraints.
            // The access policy provides add, update, and query permissions.
            SharedAccessTablePolicy sharedPolicy = new SharedAccessTablePolicy()
            {
                // Permissions enable users to add, update, query, and delete entities in the table.
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessTablePermissions.Add | SharedAccessTablePermissions.Update |
                        SharedAccessTablePermissions.Query | SharedAccessTablePermissions.Delete
            };

            try
            {
                // Get the table's existing permissions.
                TablePermissions permissions = await table.GetPermissionsAsync();

                // Add the new policy to the table's permissions, and update the table's permissions.
                permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
                await table.SetPermissionsAsync(permissions);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        /// <summary>
        /// Returns a URI containing a SAS for the table.
        /// </summary>
        /// <param name="table">A CloudTable object.</param>
        /// <param name="storedPolicyName">A string containing the name of the stored access policy. If null, an ad-hoc SAS is created.</param>
        /// <returns>A string containing the URI for the table, with the SAS token appended.</returns>
        private static string GetTableSasUri(CloudTable table, string storedPolicyName = null)
        {
            string sasTableToken;

            // If no stored policy is specified, create a new access policy and define its constraints.
            if (storedPolicyName == null)
            {
                // Note that the SharedAccessTablePolicy class is used both to define the parameters of an ad-hoc SAS, and 
                // to construct a shared access policy that is saved to the table's shared access policies. 
                SharedAccessTablePolicy adHocPolicy = new SharedAccessTablePolicy()
                {
                    // Permissions enable users to add, update, query, and delete entities in the table.
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    Permissions = SharedAccessTablePermissions.Add | SharedAccessTablePermissions.Update |
                        SharedAccessTablePermissions.Query | SharedAccessTablePermissions.Delete
                };

                // Generate the shared access signature on the table, setting the constraints directly on the signature.
                sasTableToken = table.GetSharedAccessSignature(adHocPolicy, null);

                Console.WriteLine("SAS for table (ad hoc): {0}", sasTableToken);
                Console.WriteLine();
            }
            else
            {
                // Generate the shared access signature on the table. In this case, all of the constraints for the
                // shared access signature are specified on the stored access policy, which is provided by name.
                // It is also possible to specify some constraints on an ad-hoc SAS and others on the stored access policy.
                // However, a constraint must be specified on one or the other; it cannot be specified on both.
                sasTableToken = table.GetSharedAccessSignature(null, storedPolicyName);

                Console.WriteLine("SAS for table (stored access policy): {0}", sasTableToken);
                Console.WriteLine();
            }

            // Return the URI string for the table, including the SAS token.
            return table.Uri + sasTableToken;
        }

        /// <summary>
        /// Tests a table SAS to determine which operations it allows.
        /// </summary>
        /// <param name="sasUri">A string containing a URI with a SAS appended.</param>
        /// <param name="customer">The customer entity.</param>
        /// <returns>A Task object</returns>
        private static async Task TestTableSAS(string sasUri, CustomerEntity customer)
        {
            // Try performing table operations with the SAS provided.
            // Note that the storage account credentials are not required here; the SAS provides the necessary
            // authentication information on the URI.

            // Return a reference to the table using the SAS URI.
            CloudTable table = new CloudTable(new Uri(sasUri));

            // Upsert (add/update) operations: insert an entity.
            // This operation requires both add and update permissions on the SAS.
            try
            {
                // Insert the new entity.
                customer = await InsertOrMergeEntityAsync(table, customer);

                Console.WriteLine("Add operation succeeded for SAS {0}", sasUri);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode == 403)
                {
                    Console.WriteLine("Add operation failed for SAS {0}", sasUri);
                    Console.WriteLine("Additional error information: " + e.Message);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                    throw;
                }
            }

            // Read operation: query an entity.
            // This operation requires read permissions on the SAS.
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>(customer.PartitionKey, customer.RowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                CustomerEntity customerRead = result.Result as CustomerEntity;
                if (customerRead != null)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", customerRead.PartitionKey, customerRead.RowKey, customerRead.Email, customerRead.PhoneNumber);
                }

                Console.WriteLine("Read operation succeeded for SAS {0}", sasUri);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode == 403)
                {
                    Console.WriteLine("Read operation failed for SAS {0}", sasUri);
                    Console.WriteLine("Additional error information: " + e.Message);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                    throw;
                }
            }

            // Delete operation: delete an entity.
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>(customer.PartitionKey, customer.RowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                CustomerEntity customerDelete = result.Result as CustomerEntity;
                if (customerDelete != null)
                {
                    await DeleteEntityAsync(table, customerDelete);
                }

                Console.WriteLine("Delete operation succeeded for SAS {0}", sasUri);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode == 403)
                {
                    Console.WriteLine("Delete operation failed for SAS {0}", sasUri);
                    Console.WriteLine("Additional error information: " + e.Message);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                    throw;
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Validates the connection string information in app.config and throws an exception if it looks like 
        /// the user hasn't updated this to valid values. 
        /// </summary>
        /// <returns>A CloudStorageAccount object</returns>
        private static CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            CloudStorageAccount storageAccount;
            const string Message = "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.";

            try
            {
                storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            }
            catch (FormatException)
            {
                Console.WriteLine(Message);
                Console.ReadLine();
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(Message);
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }
    }
}
