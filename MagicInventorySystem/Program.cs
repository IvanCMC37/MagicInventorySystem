using System;
using Microsoft.Extensions.Configuration;

namespace MagicInventorySystem
{
    public static class Program
    {   
        //setup for using the json file
        private static IConfigurationRoot Configuration { get; } =
            new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        public static string ConnectionString { get; } = Configuration["ConnectionString"];

        private static void Main()
        {
            MainMenu();
        }

        private static void MainMenu()
        {
            //setup in order to use the related menu functions
            var ownerMenu = new OwnerMenu();
            var franchiseHolderMenu = new FranchiseHolderMenu();
            var customerMenu = new CustomerMenu();

            while (true)
            {
                Console.Write(
@"Welcome to marvelous Magic
==========================
1.  Owner
2   Franchise Holder
3.  Customer
4.  Quit
Enter an option: ");

                var input = Console.ReadLine();

                //if user input an empty input, program will show that user is already at the bottom sub menu
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("\nYou are already at the bottom of main menu.\n");
                    continue;
                }

                //we only want the user to input a number that is within range of 1 to 4
                //if user failed, we will ask the user to give another input
                if (!int.TryParse(input, out var option) || !option.IsWithinRange(1, 4))
                {
                    Console.WriteLine("\nInvalid input.Please give another input!!!\n");
                    continue;
                }
                Console.WriteLine();

                //different options lead to different sub-menu
                switch (option)
                {
                    case 1:
                        ownerMenu.run();
                        break;
                    case 2:
                        franchiseHolderMenu.run();
                        break;
                    case 3:
                        customerMenu.run();
                        break;
                    case 4:
                        Console.WriteLine("Goodbye, see you next time!");
                        return;
                }
            }   
        }
    }
}
