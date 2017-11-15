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
using Microsoft.Azure.CosmosDB.Table;

namespace TableStorage.Model
{
    

    /// <summary>
    /// Define a Customer entity for demonstrating the Table Service. For the purposes of the sample we use the 
    /// customer's first name as the row key and last name as the partition key. In reality this would not be a good
    /// PK and RK combination as it would likely not be guaranteed to be unique which is one of the requirements for an entity. 
    /// </summary>
    public class CustomerEntity : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerEntity"/> class.
        /// Your entity type must expose a parameter-less constructor
        /// </summary>
        public CustomerEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerEntity"/> class.
        /// Defines the PK and RK.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <param name="firstName">The first name.</param>
        public CustomerEntity(string lastName, string firstName)
        {
            PartitionKey = lastName;
            RowKey = firstName;
        }

        /// <summary>
        /// Gets or sets the email address for the customer.
        /// A property for use in Table storage must be a public property of a 
        /// supported type that exposes both a getter and a setter.        
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number for the customer.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; }
    }
}
