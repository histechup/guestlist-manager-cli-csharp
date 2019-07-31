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
