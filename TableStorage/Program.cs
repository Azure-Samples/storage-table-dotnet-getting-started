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


        public static void Main(string[] args)
        {
            Console.WriteLine("Azure Table Storage - Getting Started Samples\n");
            Console.WriteLine();

            BasicSamples basicSamples = new BasicSamples();
            basicSamples.RunSamples().Wait();

            AdvancedSamples advancedSamples = new AdvancedSamples();
            advancedSamples.RunSamples().Wait();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.Read();
        }

















    }
}
