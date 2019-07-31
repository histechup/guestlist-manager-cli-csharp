# guestlist-manager-cli-csharp
**A console app written in C# that demonstrates how to use MongoDB C#/.NET Driver**

The main purpose of this article is to share code and to present some basics principles on how to implements CRUD (Create Read Update Delete) operations in a MongoDB database with C#.

---

## Context 

Because of a [**big event coming soon in my life**](https://twitter.com/theTrendyTechie/status/1141522133849391105), I set myself a challenge to create an elegant console app to manage our guestlist and track RSVPs: This is how the project **Guestlist Manager app** project started.

![alt text](https://cdn-images-1.medium.com/max/800/1*qyc8q3YyEG5wmoaOTJVCDQ.gif)

---

## Why MongoDB?
Some may wonder why I chose MongoDB over other databases. Here are the factors I considered when choosing my database:

* **Dynamic schema**: MongoDB gives flexibility to change data schema without modifying any of existing data. It's perfect for this ongoing project.
* **Manageability**: The database is user friendly and doesn't require a database administrator.
* **Speed**: It's high performing for simple queries.
* **Flexibility**: Adding new columns or fields doesn't affect existing rows or application performance.
* **Scalability**: MongoDB is horizontally scalable, which helps reduce the workload and scale your business at ease.
* **It's cheap** 😉: You can set up a MongoDB server in your own Virtual Machine or use a cloud database service like MongoDB Atlas starting for free with affordable pricing model.
* **A good opportunity for me to learn more about Mongo C# driver, MongoDB Atlas and Stitch** : MongoDB is more than a database. It's a whole ecosystem that comes with many cloud services like database-as-a-service and serverless platform.

---

## Getting our workspace ready
The complete project is available on Github here. Feel free to fork it and clone it for your own use!. In order to follow along, you will need a MongoDB database you can connect to. You can use a MongoDB database running locally, or easily create a free database using MongoDB Atlas.
* Install Visual Studio and .NET Framework
* Install MongoDB locally or Create a free database using MongoDB Atlas

---

## Creating and running our console application
1. Launch Visual Studio
1. Click Create a new project
1. In the Create a new project dialog, click Console App (.NET Core)
1. Name the project "guestlist-manager-cli-csharp"
1. Click Create to create the project

![alt text](https://cdn-images-1.medium.com/max/1200/1*QWddOKYH7YKNayCOndiPNA.gif)

Visual Studio creates a new C# Console Application project and opens the file **Programs.cs.** Replace the content of Program.cs by copying and pasting the code below into the file.

```csharp
using System;

namespace guestlist_manager_cli_csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Guestlist Manager!");

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
```

A C# console application must contain a **Main** method, in which control starts and ends. The Main method is where you create objects and execute other methods.
Press **F5** to build and run the project

![alt text](https://cdn-images-1.medium.com/max/1200/1*e04l_wP4bUAf9OPNrYWn-g.gif)

---

## Connecting to the MongoDB Database from C#

### Adding MongoDB C#/.NET Driver dependencies to our project**

What will allow us to work with the data in any MongoDB database from C# is a package called MongoDB C#/.NET Driver which creates a native interface between our application and a MongoDB server.

To install the driver, we'll go through Nuget and download the package.
1. Open the **Package Manager Console** in Visual Studio with "Tools -> Nuget Package Manager -> Package Manager Console"
1. Type: "**Install-Package MongoDB.Driver**"
1. Hit enter

![alt text](https://cdn-images-1.medium.com/max/1200/1*LR4a3NU4_m1e4GtLdzij9g.gif)


---


### Adding an entity model

- Create GuestModel.cs
1. Click **Project** -> **Add Class**
1. Type "**GuestModel.cs**" in the name field
1. Click Add to add the new class to the project
Copy and paste the following code into the GuestModel.cs file and save.

```csharp
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace guestlist_manager_cli_csharp
{
    public class GuestModel
    {
        [BsonId]  // _id
        public Guid Id { get; set; }
        public string Email { get; set; }

        [BsonElement("Name")]
        public string FullName { get; set; }
        public bool HasConfirmed { get; set; }
    }
}
```
In that class, the **Id** property:
* Is required for mapping the Common Language Runtime (CLR) object to the MongoDB collection.
* Is annotated with **[BsonId]** to designate this property as the document's primary key.
The **FullName** property is annotated with the **[BsonElement]** attribute's value of Name represents the property **Name** in the MongoDB collection


---


### Adding a MongoDB helper class

 - Create MongoHelper.cs
This class is a collection of methods for dealing with MongoDB connection and CRUD operations.
1. Click **Project** -> **Add Class**
2. Type "**MongoHelper.cs**" in the name field
3. Click **Add** to add the new class to the project
Copy and paste the following code into the **MongoHelper.cs** file and save.

```csharp
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace guestlist_manager_cli_csharp
{
    public class MongoHelper
    {
        private IMongoDatabase db;

        public MongoHelper(string connectionString, string databaseName)
        {
            //Create new database connection
            var client = new MongoClient(connectionString);
            db = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Insert new document into collection
        /// </summary>
        /// <typeparam name="T">Document data type</typeparam>
        /// <param name="collectionName">Collection name</param>
        /// <param name="document">Document</param>
        public void InsertDocument<T>(string collectionName, T document)
        {
            var collection = db.GetCollection<T>(collectionName);
            collection.InsertOne(document);
        }

        /// <summary>
        /// Load all documents in collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public List<T> LoadAllDocuments<T>(string collectionName)
        {
            var collection = db.GetCollection<T>(collectionName);

            return collection.Find(new BsonDocument()).ToList();
        }

        /// <summary>
        /// Load document by Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public T LoadDocumentById<T>(string collectionName, Guid id)
        {
            var collection = db.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("Id", id);

            return collection.Find(filter).First();
        }

        /// <summary>
        /// Update document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        /// <param name="document"></param>
        public void UpdateDocument<T>(string collectionName, Guid id, T document)
        {
            var collection = db.GetCollection<T>(collectionName);

            var result = collection.ReplaceOne(
                new BsonDocument("_id", id),
                document,
                new UpdateOptions { IsUpsert = false });
        }

        /// <summary>
        /// Insert document into collection if it does not already exist, or update it if it does
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        /// <param name="document"></param>
        public void UpsertDocument<T>(string collectionName, Guid id, T document)
        {
            var collection = db.GetCollection<T>(collectionName);

            var result = collection.ReplaceOne(
                new BsonDocument("_id", id),
                document,
                new UpdateOptions { IsUpsert = true });
        }

        /// <summary>
        /// Delete document by Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        public void DeleteDocument<T>(string collectionName, Guid id)
        {
            var collection = db.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);

        }
    }
}
```


---

### Styling the console table

 - Create ConsoleTable.cs
This class is responsible for drawing a pretty data table in our console app
1. Click **Project** -> **Add Class**
1. Type "**ConsoleTable.cs**" in the name field
1. Click **Add** to add the new class to the project
Copy and paste the following code into the ConsoleTable.cs file and save.

```csharp
//Ref: https://stackoverflow.com/questions/856845/how-to-best-way-to-draw-table-in-console-app-c

using System.Collections.Generic;
using System.Linq;

namespace guestlist_manager_cli_csharp
{
    public class ConsoleTable
    {
        private readonly string[] titles;
        private readonly List<int> lengths;
        private readonly List<string[]> rows = new List<string[]>();

        public ConsoleTable(params string[] titles)
        {
            this.titles = titles;
            lengths = titles.Select(t => t.Length).ToList();
        }

        /// <summary>
        /// Add row to table with consideration of its content length
        /// </summary>
        /// <param name="row"></param>
        public void AddRow(params object[] row)
        {
            if (row.Length != titles.Length)
            {
                throw new System.Exception($"Added row length [{row.Length}] is not equal to title row length [{titles.Length}]");
            }
            rows.Add(row.Select(o => o.ToString()).ToArray());
            for (int i = 0; i < titles.Length; i++)
            {
                if (rows.Last()[i].Length > lengths[i])
                {
                    lengths[i] = rows.Last()[i].Length;
                }
            }
        }


        /// <summary>
        /// Display pretty table on the screen
        /// </summary>
        public void Print()
        {
            lengths.ForEach(l => System.Console.Write("+-" + new string('-', l) + '-'));
            System.Console.WriteLine("+");

            string line = "";
            for (int i = 0; i < titles.Length; i++)
            {
                line += "| " + titles[i].PadRight(lengths[i]) + ' ';
            }
            System.Console.WriteLine(line + "|");

            lengths.ForEach(l => System.Console.Write("+-" + new string('-', l) + '-'));
            System.Console.WriteLine("+");

            foreach (var row in rows)
            {
                line = "";
                for (int i = 0; i < row.Length; i++)
                {
                    if (int.TryParse(row[i], out int n))
                    {
                        line += "| " + row[i].PadLeft(lengths[i]) + ' ';  // numbers are padded to the left
                    }
                    else
                    {
                        line += "| " + row[i].PadRight(lengths[i]) + ' ';
                    }
                }
                System.Console.WriteLine(line + "|");
            }

            lengths.ForEach(l => System.Console.Write("+-" + new string('-', l) + '-'));
            System.Console.WriteLine("+");
        }
    }
}
```
---

### Adding our application main menu and dialogs

 - Create DialogHelper.cs
This class is responsible for displaying the main menu and the dialogs within our app.
1. Click **Project** -> **Add Class**
1. Type "**DialogHelper.cs**" in the name field
1. Click **Add** to add the new class to the project
Copy and paste the following code into the **DialogHelper.cs** file and save.

```csharp
using System;
using System.Collections.Generic;

namespace guestlist_manager_cli_csharp
{
    public static class DialogHelper
    {
        /// <summary>
        /// Display the main menu
        /// </summary>
        /// <returns>Menu Selection</returns>
        public static int ShowMainMenu()
        {
            int choice;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(20, 0);
            Console.WriteLine("Welcome to Guest List Manager");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write(
                "Please enter your choice: \n\n" +
                "[0] Add new guest. \n" +
                "[1] Show guests list. \n" +
                "[2] Update guest info (by ID). \n" +
                "[3] Delete guest (by ID). \n" +
                "[4] Exit. \n");
            Console.WriteLine("-------------------------------");

            var entry = Console.ReadLine();
            if (!int.TryParse(entry, out choice))
            {
                choice = 5;
            }
            return choice;

        }


        /// <summary>
        /// Show current page title
        /// </summary>
        /// <param name="title"></param>
        private static void ShowHeader(string title)
        {
            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(20, 0);
            Console.WriteLine(title);
            Console.ResetColor();
            Console.WriteLine();
        }


        /// <summary>
        /// Display continue message
        /// </summary>
        public static void ShowContinueMessage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n------------------------------------------\n");
            Console.ResetColor();
            Console.WriteLine("Operation completed! \n" +
                "Press return key to continue...");
            Console.Read();
        }

        /// <summary>
        /// display 'add new guest' dialog
        /// </summary>
        /// <returns></returns>
        public static GuestModel ShowAddNewGuest()
        {
            ShowHeader("Add new guest");

            var guest = new GuestModel();

            Console.Write("Enter guest full name: ");
            guest.FullName = Console.ReadLine();

            Console.Write("Enter guest email address: ");
            guest.Email = Console.ReadLine();

            return guest;
        }


        /// <summary>
        /// Display 'show guest list' dialog
        /// </summary>
        /// <param name="guestsList"></param>
        public static void ShowGuestList(List<GuestModel> guestsList)
        {
            ShowHeader("Guests list");

            var table = new ConsoleTable("Id", "Name", "Email", "Confirmed");

            foreach (var guest in guestsList)
            {
                table.AddRow(guest.Id, guest.FullName, guest.Email, guest.HasConfirmed);
            }
            table.Print();

            ShowContinueMessage();
        }

        /// <summary>
        /// Display 'Update guest' dialog
        /// </summary>
        /// <returns></returns>
        public static string ShowUpdateGuest()
        {
            ShowHeader("Update Guest");

            Console.WriteLine("Enter guest Id: ");

            return Console.ReadLine();

        }

        /// <summary>
        /// Display 'Delete guest' dialog
        /// </summary>
        /// <returns></returns>
        public static string ShowDeleteGuest()
        {
            ShowHeader("Delete Guest");

            Console.Write("Enter guest ID: ");

            return Console.ReadLine();
        }

    }
}
```

---

### Wiring up everything in Main method 

Now everything is good to go, we can wire up everything in Main method.
Open the file **Program.cs** and replace the content with the following code.

```csharp
using System;

namespace guestlist_manager_cli_csharp
{
    class Program
    {
      
        static void Main(string[] args)
        {
            //By default for a local MongoDB instance connectionString = "mongodb://localhost:27017" 
            var connectionString = "Your_MongoDB_Connection_String"; 

            const string databaseName = "GuestDatabase";
            const string collectionName = "GuestCollection";

           

            MongoHelper database = new MongoHelper(connectionString, databaseName);

            Console.Title = "Guest List Manager";

            int menuChoice;

            do
            {
                menuChoice = DialogHelper.ShowMainMenu();
                switch (menuChoice)
                {
                    case 0: // Add new guest
                        {
                            var guest = DialogHelper.ShowAddNewGuest();

                            database.InsertDocument(collectionName, guest);

                            DialogHelper.ShowContinueMessage();
                        }
                        break;
                    case 1: // Show guest list
                        {
                            var guestsList = database.LoadAllDocuments<GuestModel>(collectionName);
                            DialogHelper.ShowGuestList(guestsList);

                        }
                        break;
                    case 2: // Update guest info (by ID)
                        {
                            var guestId = DialogHelper.ShowUpdateGuest();
                            Guid guestIdGuid;
                            bool isValidGuid = Guid.TryParse(guestId, out guestIdGuid);
                            if (isValidGuid)
                            {
                                var guest = database.LoadDocumentById<GuestModel>(collectionName, guestIdGuid);
                                Console.Write($"\n" +
                                    $"Current info\n" +
                                    $"Name: {guest.FullName} \n" +
                                    $"Email: {guest.Email}");

                                Console.WriteLine("\n------------------------------------------\n");

                                Console.WriteLine("Enter new full name (leave empty if no changes");
                                var fullName = Console.ReadLine();

                                if (!string.IsNullOrEmpty(fullName) && !string.IsNullOrWhiteSpace(fullName))
                                {
                                    guest.FullName = fullName;
                                }

                                Console.WriteLine("Enter new email (leave empty if no changes");
                                var email = Console.ReadLine();

                                if (!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email))
                                {
                                    guest.Email = email;
                                }

                                database.UpdateDocument<GuestModel>(collectionName, guestIdGuid, guest);
                            }
                            else
                            {
                                Console.WriteLine($"Exception: '{guestId}' is not a valid Guid!");
                            }

                            DialogHelper.ShowContinueMessage();
                        }
                        break;
                    case 3: // Delete guest (by ID)
                        {
                            var guestId = DialogHelper.ShowDeleteGuest();

                            Guid guestIdGuid;

                            bool isValidGuid = Guid.TryParse(guestId, out guestIdGuid);
                            if (isValidGuid)
                            {
                                database.DeleteDocument<GuestModel>(collectionName, guestIdGuid);
                            }
                            else
                            {
                                Console.WriteLine($"Exception: '{guestId}' is not a valid Guid!");
                            }


                            DialogHelper.ShowContinueMessage();
                        }
                        break;
                }

            } while (menuChoice != 4);


        }


    }
}
```


---

### Setting the connection string of our database

In **Main** method, replace the value of the connection string with your MongoDB server.

```csharp
//By default for a local MongoDB instance connectionString = "localhost:27017"
var connectionString = "Your_MongoDB_Connection_String";
```

To get your connection string from **MongoDB Atlas** you can follow the instructions from the official documentation: [Connect via Driver](https://docs.atlas.mongodb.com/driver-connection/).


---

### That's all, our application is ready to run!

Press **F5** to build and run the project

>Even if you don't have any existing database named "GuestDatabase" and or a collection named "GuestCollection", MongoDB will create it for you while inserting your first document.

[**Here is the git repository link for the complete project.**](https://github.com/histechup/guestlist-manager-cli-csharp)


---

## Wrapping up

Now, we have a fully functional **Guestlist Manager** console app. We can create, read, update and delete our guest records stored in a MongoDB database. Here is the recap of what we've covered:
* Create a C# console application using Visual Studio.
* Connect our application to MongoDB.
* Learn how to execute CRUD operations by using MongoDB C#/.NET Driver.


---

## Next steps

And there we have it! Our **Guestlist Manager** currently lets us manage the list directly. BUT that's only half the battle - wouldn't it be cool if we could also send invitations directly from the database? In my next post, I'll take this a step further and show you how to do this with [**MongoDB Stitch**] and [**SendGrid**].



---

## Resources

* [What is MongoDB - from the official source](https://www.mongodb.com/what-is-mongodb)
* [Learn more about MongoDB C#/.NET Driver](https://docs.mongodb.com/ecosystem/drivers/csharp/)
* [Learn more about MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
* [How to create C# apps with Visual Studio](https://docs.microsoft.com/en-us/visualstudio/get-started/csharp/?view=vs-2019)


---

## Feedback

Thanks for reading! I hope this helped you with your first C# and MongoDB project. I kept this project intentionally simple, and there are many ways it can be built upon and improved. Please feel free to leave a comment here or create an issue in the Github repo - this will help me continue to create content that is most helpful to you and the community. Til next time!




