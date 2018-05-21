using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace MagicInventorySystem
{
    //this class is to manage the sql operation between ownerMenu calss and database
    public class OwnerManager
    {
        //create list to store data from database
        public List<Inventory> Items { get; set; }
        public List<Request> Requests { get; set; }

        public OwnerManager()
        {
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                //setup to use store procedure from database - get all owner items
                var command = connection.CreateCommand();
                command.CommandText = "SelectAllOwnerItems";
                command.CommandType = CommandType.StoredProcedure;

                //store the data into item list
                Items = command.GetDataTable().Select().Select(x =>
                    new Inventory((int)x["ProductID"], (string)x["Name"], (int)x["StockLevel"])).ToList();
            }
        }

        //function to return the related Inventory item when user gave a productID
        public Inventory GetItem(int productID) => Items.FirstOrDefault(x => x.ProductID == productID);

        //fucntion to return the related request when user gave a requestID
        public Request GetRequest(int requestID) => Requests.FirstOrDefault(x => x.RequestID == requestID);

        //function to process the sql request (update owner's item stock) 
        public void UpdateItemStock(Inventory item)
        {
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                //setup to use the store procedure on database
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UpdateOwnerStock";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@pID",SqlDbType.Int).Value = item.ProductID;
                command.Parameters.Add("@sLevel", SqlDbType.Int).Value = item.StockLevel;
                command.ExecuteNonQuery();
            }
        }

        //funtion to process the sql operation (get all request) as a list  
        public void GetItemRequest()
        {
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                //setup to use the store procedure on database
                var command = connection.CreateCommand();
                command.CommandText = "GetAllRequests";
                command.CommandType = CommandType.StoredProcedure;

                //store the data into request list
                Requests = command.GetDataTable().Select().Select(x =>
                new Request((int)x["ID"], (string)x["Store"], (string)x["Product"], (int)x["Quantity"], (int)x["Current Stock"], (string)x["Stock Availability"], (int)x["ProductID"], (int)x["StoreID"])).ToList();
            }
        }

        //SQL part  for Processing request 
        public void DeleteRequest(Request request)
        {
            using (var connection = Program.ConnectionString.CreateConnection())
            {
                //setup to use the store procedure on database
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DeleteRequest";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@rID", SqlDbType.Int).Value = request.RequestID;
                command.ExecuteNonQuery();
            }
        }
    }
}
