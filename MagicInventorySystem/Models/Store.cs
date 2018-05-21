namespace MagicInventorySystem
{
    public class Store
    {
        public int StoreID { get; }
        public string Name { get; }

        public Store(int storeID, string name)
        {
            StoreID = storeID;
            Name = name;
        }
    }
}
