using System;
using System.Linq;

namespace MagicInventorySystem
{
    public class CustomerMenu
    {
        //setup in order to use the related sql-side functions
        private FranchiseHolderManager FranchiseHolderManager { get; } = new FranchiseHolderManager();

        //setup for the side-show menu later on
        int startPoint, endPoint, step, div, mod, printCount;

        //variable for let user back to main menu
        bool chooseToReturn = false;

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
                    Console.WriteLine("Invalid input.");
                    Console.WriteLine();
                    continue;
                }

                //store user choice of store in order to proceed later on
                FranchiseHolderManager.StoreID = option;

                //call function for sub menu
                SubRetailMenu();
            }
        }

        //function to display all the store
        private void DisplayStores()
        {
            //if store list is empty, function will not preceed further
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

        //function of sub menu after user chose a store
        private void SubRetailMenu()
        {
            while (true)
            {
                //use the get store function at franchiseholderManager to get information of the related store
                var store = FranchiseHolderManager.GetStore(FranchiseHolderManager.StoreID);

                Console.Write(
$@"Welcome to Marvelous Magic (Retail - {store.Name})
===================================
1.  Display Product
2.  Return to Main Menu

Enter an option: ");
                var input = Console.ReadLine();
                Console.WriteLine();

                //if user input an empty input, program will show that user is already at the bottom sub menu
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("You are already at the bottom of sub menu.\n");
                    continue;
                }

                //making sure that user only input a number and within range of 1 to 2
                if (!int.TryParse(input, out var option) || !option.IsWithinRange(1, 2))
                {
                    Console.WriteLine("Invalid input.\n");
                    continue;
                }

                //related function depends on user input
                switch (option)
                {
                    case 1:
                        FranchiseHolderManager.GetStoreStock();
                        DisplayProducts();
                        break;
                    case 2:
                        chooseToReturn = true;
                        return;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        //function to display product in page(s)
        private void DisplayProducts()
        {
            //each page only will show 3 items max
            step = 3;

            //setup of the list size
            startPoint = 0;
            endPoint = 0;
            div = FranchiseHolderManager.Items.Count() / step;
            mod = FranchiseHolderManager.Items.Count() % step;

            //call print product menu function
            PrintFlipProductMenu();

            while (true)
            {
                Console.Write("Enter product ID to purchase or function: ");
                var input = Console.ReadLine();
                Console.WriteLine();

                //if "N" is entered and the list still has item(s) to go though
                if (input == "N" && div>=0)
                    PrintFlipProductMenu();
                
                //go back to menu if user entered "R"
                else if (input == "R")
                    break;
                else
                {
                    //user input "N" but list had already reached the end
                    if (input == "N")
                    {
                        Console.WriteLine("You can't go next page since you had reached the end of the list.\n");
                        continue;
                    }
                    //user input a non- number to buy a product
                    if (!int.TryParse(input, out var id))
                    {
                        Console.WriteLine("Invalid input.\n");
                        continue;
                    }

                    //user input an item id that doesn't present in the store
                    var item = FranchiseHolderManager.GetItem(id);
                    if (item == null || id>endPoint || id<=endPoint-printCount)
                    {
                        Console.WriteLine("No such item.\n");
                        continue;
                    }

                    //input passed all conditions above, proceeed to buy
                    BuyItem(item);
                    break;
                }   
            }
        }

        //print product menu function
        private void PrintFlipProductMenu()
        {
            //if store doesn't have any item, function will not proceed further
            if (!FranchiseHolderManager.Items.Any())
            {
                Console.WriteLine("No items present at selected store.\n");
                return;
            }

            //go though all items as a list
            if (!(mod == 0) || !(div == 0))
            {
                if (div == 0)
                {
                    endPoint += mod;
                    mod = 0;
                }
                //the product list had already reached the end
                else if (div < 0)
                    return;
                else
                    endPoint += step;
                Console.WriteLine("Inventory\n");
                const string format = "{0,-5}{1,-25}{2}\n";
                Console.WriteLine(format, "ID", "Product", "Current Stock");
                printCount = 0;
                
                //print the list in group of 3 maximum
                for (int x = startPoint; x < endPoint; x++)
                {
                    printCount++;
                    Console.WriteLine(format, FranchiseHolderManager.Items[x].ProductID, FranchiseHolderManager.Items[x].Name, FranchiseHolderManager.Items[x].StockLevel);
                }
                div--;
                startPoint = endPoint;
                Console.WriteLine("[Legend: 'N' Next Page | 'R' Return to Menu]\n");      
            }
        }

        //function to buy the item
        private void BuyItem(Inventory item)
        { 
            while (true)
            {
                Console.Write("Enter quantity to purchase : ");

                var input = Console.ReadLine();
                Console.WriteLine();

                //if user input an empty input, program will go back to sub menu
                if (string.IsNullOrEmpty(input))
                    break;
                
                //user can only input a number
                if (!int.TryParse(input, out var amount))
                {
                    Console.WriteLine("Invalid input.\n");
                    continue;
                }

                //prodcut cannot be bought since store doesn't have the enough stock
                if (amount > item.StockLevel)
                {
                    Console.WriteLine($"{item.Name} doesn't have enough stock to fulfil the purchase.\n");
                    continue;
                }

                //user cant buy quantity <=0
                if (amount <= 0)
                {
                    Console.WriteLine("Amount can't be less than 1, please try again.\n");
                    continue;
                }

                //update to item list
                item.StockLevel -= amount;

                //update to database from using related sql operation
                FranchiseHolderManager.UpdateItemStock(item);
                Console.WriteLine($"Purchased {amount} of {item.Name}.\n");
                break;
            }
        }
    }
}
