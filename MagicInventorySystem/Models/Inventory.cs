namespace MagicInventorySystem
{
    public class Inventory
    {
        public int ProductID { get; }
        public string Name { get; }
        public int StockLevel { get; set; }

        public Inventory(int productID, string name, int stockLevel)
        {
            ProductID = productID;
            Name = name;
            StockLevel = stockLevel;
        }
    }
}
