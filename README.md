---
services: storage
platforms: dotnet
author: jasonnewyork
---

# Getting Started with Azure Table Storage in .NET

This demo demonstrates how to perform common tasks using Azure Table storage 
and Azure Cosmos DB Table API including creating a table, CRUD operations, 
batch operations and different querying techniques. 

If you don't have a Microsoft Azure subscription you can get a FREE trial 
account [here](http://go.microsoft.com/fwlink/?LinkId=330212)

Note: This sample uses the .NET 4.5 asynchronous programming model to demonstrate how to call the Storage Service using the
storage client libraries asynchronous API's. When used in real applications this approach enables you to improve the
responsiveness of your application. Calls to the storage service are prefixed by the await keyword.
If you don't have a Microsoft Azure subscription you can
get a FREE trial account [here](http://go.microsoft.com/fwlink/?LinkId=330212)

## Running this sample

### Azure Cosmos DB Table API

__NOTE: This sample will not work against tables in table account created during the preview period. It will only work against
tables created in table accounts created after GA on 11/15/2017.__

1. Go to your Azure Cosmos DB Table API instance in the Azure Portal and select 
"Connection String" in the menu, select the Read-write Keys tab and copy the value 
in the "CONNECTION STRING" field.
2. Open the App.config file and set 
StorageConnectionString to your connection string.
3. Load the project in Visual Studio (the [community version](https://www.visualstudio.com/vs/community/) is available for free)
4. Run

#### More Information
-[Introduction to Azure Cosmos DB Table API](https://docs.microsoft.com/en-us/azure/cosmos-db/table-introduction)

### Azure Table storage

This sample can be run using either the Azure Storage Emulator that installs as part of this SDK - or by
updating the App.Config file with your connection string.


To run the sample using the Storage Emulator (default option):

1. Download and Install the Azure Storage Emulator [here](http://azure.microsoft.com/en-us/downloads/).
2. Start the Azure Storage Emulator (once only) by pressing the Start button or the Windows key and searching for it by
typing "Azure Storage Emulator". Select it from the list of applications to start it.
3. Open App.config and set the value of StorageConnectionString to "UseDevelopmentStorage=true;"
4. Load the project in Visual Studio (the [community version](https://www.visualstudio.com/vs/community/) is available for free)
5. Run


To run the sample using the Storage Service:

1. Go to your Azure Storage account in the Azure Portal and under "SETTINGS" 
click on "Access keys". Copy either key1 or key2's "CONNECTION STRING".
2. Open the app.config file and comment out the connection string for the emulator (UseDevelopmentStorage=True) and uncomment the connection string for the storage service (AccountName=[]...)
3. Load the project in Visual Studio (the [community version](https://www.visualstudio.com/vs/community/) is available for free)
4. Run

#### More information
- [What is a Storage Account](http://azure.microsoft.com/en-us/documentation/articles/storage-whatis-account/)
- [Getting Started with Tables](http://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-tables/)
- [Table Service Concepts](http://msdn.microsoft.com/en-us/library/dd179463.aspx)
- [Table Service REST API](http://msdn.microsoft.com/en-us/library/dd179423.aspx)
- [Table Service C# API](http://go.microsoft.com/fwlink/?LinkID=398944)
- [Storage Emulator](http://msdn.microsoft.com/en-us/library/azure/hh403989.aspx)
- [Asynchronous Programming with Async and Await](http://msdn.microsoft.com/en-us/library/hh191443.aspx)
