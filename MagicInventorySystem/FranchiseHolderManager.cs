using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace MagicInventorySystem
{
    //this class is to manage the sql operation between franchiseholder calss and database
    public class FranchiseHolderManager
    {
        //create list to store data from database
        public List<Inventory> Items { get; set; }
        public List<Store> Stores { get; set; }

        //setup for storing user's choice of store later
        public int StoreID { set; get; }

        public FranchiseHolderManager()
        {
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                //setup to use store procedure from database - get list of  stores
                var command = connection.CreateCommand();
                command.CommandText = "SelectAllStore";
                command.CommandType = CommandType.StoredProcedure;

                //store the data into store list
                Stores = command.GetDataTable().Select().Select(x =>
                    new Store((int)x["StoreID"], (string)x["Name"])).ToList();
            }
        }

        //function to return the related store when user gave a store
        public Store GetStore(int StoreID) => Stores.FirstOrDefault(x => x.StoreID == StoreID);

        //function to return the related inventory item when user gave a productID
        public Inventory GetItem(int ProductID) => Items.FirstOrDefault(x => x.ProductID == ProductID);

        //function to call the sql operation for updating the list of item of the store
        public void GetStoreStock()
        {
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                //setup to use store procedure from database
                var command = connection.CreateCommand();
                command.CommandText = "SelectAllStoreItems";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@sID", SqlDbType.Int).Value = StoreID;

                //store data to item list
                Items = command.GetDataTable().Select().Select(x =>
                    new Inventory((int)x["ProductID"], (string)x["Name"], (int)x["StockLevel"])).ToList();
            }
        }

        //fucntion to call the sql operation to get the filtered list after user gave a threshold
        public void GetFilterStoreStock(int threshold)
        {
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                //setup to use store procedure from database
                var command = connection.CreateCommand();
                command.CommandText = "GetFilterStoreStock";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@sID", SqlDbType.Int).Value = StoreID;
                command.Parameters.Add("@max", SqlDbType.Int).Value = threshold;

                //store data to item list
                Items = command.GetDataTable().Select().Select(x =>
                    new Inventory((int)x["ProductID"], (string)x["Name"], (int)x["StockLevel"])).ToList();
            }
        }

        //function to call the sql operation to insert item request
        public void InsertRequest(Inventory item, int threshold)
        {
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                //setup to use store procedure from database
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "InsertRequestToOwner";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@sID", SqlDbType.Int).Value = StoreID;
                command.Parameters.Add("@pID", SqlDbType.Int).Value = item.ProductID;
                command.Parameters.Add("@amount", SqlDbType.Int).Value = threshold;
                command.ExecuteNonQuery();
            }
        }

        //function to call the sql operation to update stock for the franchiseholder
        public void UpdateItemStock(Inventory item)
        {
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                //setup to use store procedure from database
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UpdateHolderStock";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@sLevel", SqlDbType.Int).Value = item.StockLevel;
                command.Parameters.Add("@pID", SqlDbType.Int).Value = item.ProductID;
                command.Parameters.Add("@sID", SqlDbType.Int).Value = StoreID;
                command.ExecuteNonQuery();
            }
        }

        //function to call the sql operation to get the list of item that the franchiseholder doesn't have
        public void AddNewItem()
        {   
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                //setup to use store procedure from database
                var command = connection.CreateCommand();
                command.CommandText = "GetNewItemFromOwner";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@sID", SqlDbType.Int).Value = StoreID;

                //store data to item list
                Items = command.GetDataTable().Select().Select(x =>
                    new Inventory((int)x["ProductID"], (string)x["Name"], (int)x["StockLevel"])).ToList();
            }
        }

        //fucntion to call the sql operation to add a new item row related store
        public void InsertNewItem(Inventory item)
        {   
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                //setup to use store procedure from database
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "InsertNewItem";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@sID", SqlDbType.Int).Value = StoreID;
                command.Parameters.Add("@pID", SqlDbType.Int).Value = item.ProductID;
                command.ExecuteNonQuery();
            }
        }
    }
}

  
