using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MagicInventorySystem
{
    //this class is for all the shared utilities between classes
    public static class CustomUtilities
    {
        //function for checking if user input is within range
        public static bool IsWithinRange(this int value, int min, int max) => value >= min && value <= max;

        //function for coonecting to sql server from using connectionString
        public static SqlConnection CreateConnection(this string connectionString) =>
    new SqlConnection(connectionString);

        //function to return data table by using dataAdapter
        public static DataTable GetDataTable(this SqlCommand command)
        {
            var table = new DataTable();
            new SqlDataAdapter(command).Fill(table);
            return table;
        }

        //function to print the item list
        public static void DisplayItems(IEnumerable<Inventory> items)
        {
            const string format = "{0,-5}{1,-25}{2}";
            Console.WriteLine(format, "ID", "Product", "Current Stock");
            foreach (var x in items)
            {
                Console.WriteLine(format, x.ProductID, x.Name, x.StockLevel);
            }
            Console.WriteLine();
        }

        //function to print the store list
        public static void DisplayStores(IEnumerable<Store> stores)
        {
            const string format = "{0,-4}{1}";
            Console.WriteLine(format, "ID", "ID", "Name");
            foreach (var x in stores)
            {
                Console.WriteLine(format, x.StoreID, x.Name);
            }
        }

    }
}
