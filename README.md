---
services: storage
platforms: dotnet
author: jasonnewyork
---

# Getting Started with Azure Table Storage in .NET

Demonstrates how to perform common tasks using the Microsoft Azure Table storage
including creating a table, CRUD operations, batch operations and different querying techniques.

Note: This sample uses the .NET 4.5 asynchronous programming model to demonstrate how to call the Storage Service using the
storage client libraries asynchronous API's. When used in real applications this approach enables you to improve the
responsiveness of your application. Calls to the storage service are prefixed by the await keyword.
If you don't have a Microsoft Azure subscription you can
get a FREE trial account [here](http://go.microsoft.com/fwlink/?LinkId=330212)

## Running this sample

This sample can be run using either the Azure Storage Emulator that installs as part of this SDK - or by
updating the App.Config file with your AccountName and Key.
To run the sample using the Storage Emulator (default option):

1. Download and Install the Azure Storage Emulator [here](http://azure.microsoft.com/en-us/downloads/).
2. Start the Azure Storage Emulator (once only) by pressing the Start button or the Windows key and searching for it by typing "Azure Storage Emulator". Select it from the list of applications to start it.
3. Set breakpoints and run the project using F10.

To run the sample using the Storage Service

1. Open the app.config file and comment out the connection string for the emulator (UseDevelopmentStorage=True) and uncomment the connection string for the storage service (AccountName=[]...)
2. Create a Storage Account through the Azure Portal and provide your [AccountName] and [AccountKey] in the App.Config file.
3. Set breakpoints and run the project using F10.

## More information
- [What is a Storage Account](http://azure.microsoft.com/en-us/documentation/articles/storage-whatis-account/)
- [Getting Started with Tables](http://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-tables/)
- [Table Service Concepts](http://msdn.microsoft.com/en-us/library/dd179463.aspx)
- [Table Service REST API](http://msdn.microsoft.com/en-us/library/dd179423.aspx)
- [Table Service C# API](http://go.microsoft.com/fwlink/?LinkID=398944)
- [Storage Emulator](http://msdn.microsoft.com/en-us/library/azure/hh403989.aspx)
- [Asynchronous Programming with Async and Await](http://msdn.microsoft.com/en-us/library/hh191443.aspx)
