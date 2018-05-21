using System;
using System.Linq;

namespace MagicInventorySystem
{
    public class FranchiseHolderMenu
    {
        //variable for let user back to main menu
        bool chooseToReturn = false;

        //setup in order to use the related sql-side functions
        private FranchiseHolderManager FranchiseHolderManager { get; } = new FranchiseHolderManager();
        public void run()
        {
            while (true)
            {
                //boolean check in order to quit to main menu
                if (chooseToReturn == true)
                {
                    chooseToReturn = false;
                    return;
                }

                //print the store list from database
                DisplayStores();
                var input = Console.ReadLine();
                Console.WriteLine();

                //if user input an empty input, program will go back to sub menu
                if (string.IsNullOrEmpty(input))
                    break;

                //user input cannot be non-number and number not in range of 1 to 5
                if (!int.TryParse(input, out var option) || !option.IsWithinRange(1, 5))
                {
                    Console.WriteLine("Invalid input.\n");
                    continue;
                }

                //store user choice of store in order to proceed later on
                FranchiseHolderManager.StoreID = option;

                //call function for sub menu
                SubStoreMenu();
            }
        }

        //function of menu after selecting store
        private void SubStoreMenu()
        {
            while(true)
            {
                //use the get store function at franchiseholderManager to get information of the related store
                var store = FranchiseHolderManager.GetStore(FranchiseHolderManager.StoreID);

                Console.Write(
$@"Welcome to Marvelous Magic (Franchise Holder - {store.Name})
===================================
1.  Display Inventory
2.  Stock Request (Threshold)
3.  Add New Inventory Item
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

                //user input cannot be non-number and number not in range of 1 to 4
                if (!int.TryParse(input, out var option) || !option.IsWithinRange(1, 4))
                {
                    Console.WriteLine("Invalid input.\n");
                    continue;
                }

                //related function depends on user input
                switch (option)
                {
                    case 1:
                        FranchiseHolderManager.GetStoreStock();
                        DisplayItems();
                        break;
                    case 2:
                        StockRequest();
                        break;
                    case 3:
                        AddNewItem();
                        break;
                    case 4:
                        chooseToReturn = true;
                        return;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        //function to display all stores
        private void DisplayStores()
        {
            //if store from inventory cannot be fetched/ empty, the operation will not be continued in order to prevent crash
            if (!FranchiseHolderManager.Stores.Any())
            {
                Console.WriteLine("No stores present.\n");
                return;
            }
            Console.WriteLine("Stores\n");

            //using the shared function from customutilities class to display the store list
            CustomUtilities.DisplayStores(FranchiseHolderManager.Stores);
            Console.Write("Enter the store to use: ");
        }

        //function to display the inventory of the store
        private void DisplayItems()
        {
            //if the store does not have any item, the operation will not be continued in order to prevent crash
            if (!FranchiseHolderManager.Items.Any())
            {
                Console.WriteLine("No items present at selected store.\n");
                return;
            }
            Console.WriteLine("Inventory\n");

            //using the shared function from customutilities class to display the item list
            CustomUtilities.DisplayItems(FranchiseHolderManager.Items);
        }

        //function for store to request stock 
        private void StockRequest()
        {
            while (true)
            {
                Console.Write("Enter threshold for re-stocking: ");
                var input = Console.ReadLine();
                Console.WriteLine();

                //if user input an empty input, program will go back to sub menu
                if (string.IsNullOrEmpty(input))
                    break;
                
                //only number inut will be accepted
                if (!int.TryParse(input, out var threshold))
                {
                    Console.WriteLine("Invalid input.\n");
                    continue;
                }

                //user can only request with threshold >1
                if (threshold<=1)
                {
                    Console.WriteLine("All inventory stock level are equal to or above 1\n");
                    continue;
                }

                //call the related sql operation to store the filtered item list
                FranchiseHolderManager.GetFilterStoreStock(threshold);

                //call the function to display the list
                DisplayItems();

                //if no item meet the thresold request, function will not proceed further
                if(FranchiseHolderManager.Items.Any())
                    StockRequest(threshold);
                break;
            }
        }

        //function for 2nd part of requsting stock
        private void StockRequest(int threshold)
        {
            while (true)
            {
                Console.Write("Enter request to proceess: ");
                var input = Console.ReadLine();
                Console.WriteLine();

                //if user input an empty input, program will go back to sub menu
                if (string.IsNullOrEmpty(input))
                    break;

                //only number inut will be accepted
                if (!int.TryParse(input, out var id))
                {
                    Console.WriteLine("Invalid input.");
                    Console.WriteLine();
                    continue;
                }

                //if input productID is not on the related item list, new input will be requested
                var item = FranchiseHolderManager.GetItem(id);
                if (item == null)
                {
                    Console.WriteLine("No such item.\n");
                    continue;
                }

                //input passed all conditions above, calling related sql operation
                FranchiseHolderManager.InsertRequest(item,threshold);
                Console.WriteLine("Stock request created.\n");
                break;
            }
        }

        //function to add new item to store
        private void AddNewItem()
        {
            //update the item list to reflect the item that store doesn't have
            FranchiseHolderManager.AddNewItem();
            DisplayItems();
            while (true)
            {
                //if list is empty, function will not proceed further
                if (!FranchiseHolderManager.Items.Any())
                    break;
                
                Console.Write("Enter item to add: ");
                var input = Console.ReadLine();
                Console.WriteLine();

                //if user input an empty input, program will go back to sub menu
                if (string.IsNullOrEmpty(input))
                    break;
                
                //user can only input number
                if (!int.TryParse(input, out var id))
                {
                    Console.WriteLine("Invalid input.\n");
                    continue;
                }

                //call getItem check and if the item not on the list, a new input will be requested
                var item = FranchiseHolderManager.GetItem(id);
                if (item == null)
                {
                    Console.WriteLine("No such item.\n");
                    continue;
                }

                //input passed all conditions above, a request of that related item will be request to owner
                FranchiseHolderManager.InsertRequest(item, 1);
                FranchiseHolderManager.InsertNewItem(item);
                Console.WriteLine($"Created request for Product - {item.Name}.\n");
                break;
            }
        }
    }
}
