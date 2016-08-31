using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using TableStorage.Model;

namespace TableStorage
{
    public class BasicSamples
    {
        public async Task RunSamples()
        {
            Console.WriteLine("Azure Table Storage - Basic Samples\n");
            Console.WriteLine();

            string tableName = "demo" + Guid.NewGuid().ToString().Substring(0, 5);

            // Create or reference an existing table
            CloudTable table = await Common.CreateTableAsync(tableName);

            // Demonstrate basic CRUD functionality 
            await BasicDataOperationsAsync(table);

            // Create a SAS and try CRUD operations with the SAS.
            await BasicDataOperationsWithSasAsync(table);

            // Delete the table
            await table.DeleteIfExistsAsync();
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

    }
}