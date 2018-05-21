namespace MagicInventorySystem
{
    public class Request
    {
        public int RequestID { get; }
        public string StoreName { get; }
        public string ProductName { get; }
        public int Quantity { get; }
        public int CurrentStock { get; set; }
        public string StockAvailabilty { get; }
        public int ProductID { get; }
        public int StoreID { get; }

        public Request(int requestID, string storeName, string productName, int quantity, int currentStock, string stockAvailability, int productID, int storeID)
        {
            RequestID = requestID;
            StoreName = storeName;
            ProductName = productName;
            Quantity = quantity;
            CurrentStock = currentStock;
            StockAvailabilty = stockAvailability;
            ProductID = productID;
            StoreID = storeID;
        }
    }
}
