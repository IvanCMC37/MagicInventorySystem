using System;
using System.Linq;

namespace MagicInventorySystem
{
    public class OwnerMenu
    {
        //setup in order to use the related sql-side functions
        private OwnerManager OwnerManager { get; } = new OwnerManager();
        private FranchiseHolderManager FranchiseHolderManager { get; } = new FranchiseHolderManager();

        public void run()
        {
            while (true)
            {
                Console.Write(
@"Welcome to Marbelous Magic (Owner)
===================================
1.  Display All Stock Requests
2.  Display  Owner Inventory
3.  Reset Inventory Item Stock
4.  Return to Main Menu

Enter an option: ");

                var input = Console.ReadLine();
                Console.WriteLine();

                //if user input an empty input, program will show that user is already at the bottom sub menu
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("You are already at the bottom of sub menu.\n");
                    continue;
                }

                //if user input a non number or number is not within the range of 1 to 4, a re-enter will be requested
                if (!int.TryParse(input, out var option) || !option.IsWithinRange(1, 4))
                {
                    Console.WriteLine("Invalid input.\n");
                    continue;
                }

                //call different function depends on input
                switch (option)
                {
                    case 1:
                        DisplayAllRequests();
                        break;
                    case 2:
                        DisplayAllItems();
                        break;
                    case 3:
                        ResetStock();
                        break;
                    case 4:
                        return;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        //this function is for option 1 -  Display All Stock Requests
        private void DisplayAllRequests()
        {
            //call the related sql operation to update the List of "items"
            OwnerManager.GetItemRequest();
            Console.WriteLine("Stock Requests\n");

            //if no request was found, dont need to proceed further
            if (!OwnerManager.Requests.Any())
            {
                Console.WriteLine("No request present.\n");
                return;
            }

            //print out all the request from stores
            const string format = "{0,-5}{1,-16}{2,-22}{3,-10}{4,-15}{5}";
            Console.WriteLine(format, "ID", "Store", "Product","Quantity","Current Stock", "Stock Availability");
            foreach (var x in OwnerManager.Requests)
            {
                Console.WriteLine(format, x.RequestID, x.StoreName, x.ProductName,x.Quantity,x.CurrentStock,x.StockAvailabilty);
            }

            //call the process request in order to let the owner to fill the request 1 by 1
            ProcessRequest();
        }

        //function to process input form owner
        private void ProcessRequest()
        {
            while (true)
            {
                Console.Write("\nEnter request to process: ");
                var input = Console.ReadLine();
                Console.WriteLine();
                //if the user give a null input, then no request will be processed and the menu will go back to owner sub menu
                if (string.IsNullOrEmpty(input))
                    break;

                //a new input will be requested if user entered a non-number
                if (!int.TryParse(input, out var id))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                //if user entered an id that  is not on the current list and new input will be requested
                var request = OwnerManager.GetRequest(id);
                if (request == null)
                {
                    Console.WriteLine("No such request.");
                    continue;
                }
                
                //process the request if availability is true and passed all the conditions above
                if (request.StockAvailabilty.Equals("True"))
                {
                    //setup to use the related sql operation later on
                    var ownerItem = OwnerManager.GetItem(request.ProductID);
                    ownerItem.StockLevel -= request.Quantity;
                    OwnerManager.UpdateItemStock(ownerItem);
                    FranchiseHolderManager.StoreID = request.StoreID;
                    FranchiseHolderManager.GetStoreStock();
                    var holderItem = FranchiseHolderManager.GetItem(request.ProductID);
                    holderItem.StockLevel += request.Quantity;

                    //finish up the request in 3 parts: 1 - update owner stock, 2 - update franchiseholder stock, 3- delete the request from database
                    FranchiseHolderManager.UpdateItemStock(holderItem);
                    OwnerManager.DeleteRequest(request);
                    Console.WriteLine($"Filled request - {request.ProductName} for store - {request.StoreName}.\n");
                }
                else
                {
                    Console.WriteLine("You do not have enough stock to fulfil the request.\n");
                    continue;
                }
                break;
            }
        }

        //function to display owner's inventory 
        private void DisplayAllItems()
        {
            Console.WriteLine("Owner Inventory\n");

            //if items from inventory cannot be fetched, the operation will not continue in order to prevent crash
            if (!OwnerManager.Items.Any())
            {
                Console.WriteLine("No items present.\n");
                return;
            }

            //using the shared function from customutilities class to display the item list
            CustomUtilities.DisplayItems(OwnerManager.Items);
        }

        //function to restock item for owner
        private void ResetStock()
        {
            Console.WriteLine("Reset Stock\nProduct stock will be reset to 20.\n");

            //if items from inventory cannot be fetched, the operation will not continue in order to prevent crash
            if (!OwnerManager.Items.Any())
            {
                Console.WriteLine("No items present.\n");
                return;
            }

            //using the shared function from customutilities class to display the item list
            CustomUtilities.DisplayItems(OwnerManager.Items);

            while (true)
            {
                Console.Write("Enter item ID to reset: ");
                var input = Console.ReadLine();
                Console.WriteLine();

                //if the user give a null input, then no request will be processed and the menu will go back to owner sub menu
                if (string.IsNullOrEmpty(input))
                    break;
                
                //if user entered a non-number a new input will be requested
                if (!int.TryParse(input, out var id))
                {
                    Console.WriteLine("Invalid input.\n");
                    continue;
                }

                //if user entered an id that is not on the current list and new input will be requested
                var item = OwnerManager.GetItem(id);
                if (item == null)
                {
                    Console.WriteLine("No such item.\n");
                    continue;
                }

                //input passed all conditions above and restock function will be called
                ResetStock(item);
                break;
            }
        }

        //fucntion to process the restock of inventory
        private void ResetStock(Inventory item)
        {
            //making sure that restock will only happen when stock level < 20
            if (item.StockLevel >= 20)
                Console.WriteLine($"{item.Name} already has enough stock.\n");
            else
            {
                item.StockLevel = 20;
                
                //call the related sql function to update the database
                OwnerManager.UpdateItemStock(item);
                Console.WriteLine($"{item.Name} stock level has been reset to {item.StockLevel}.\n");
            }
        }
    }
}
