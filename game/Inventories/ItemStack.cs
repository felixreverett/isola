namespace Isola.Inventories
{
    public struct ItemStack
    {
        public string ItemName { get; set; }
        public int Amount { get; set; }
        public ItemStack(string itemName, int amount)
        {
            this.ItemName = itemName;
            this.Amount = amount;
        }

        public ItemStack(List<object> itemStackData)
        {
            this.ItemName = (string)itemStackData[0];
            this.Amount = (int)itemStackData[1];
        }

        // For entity saving
        public List<object> GetSaveData()
        {
            return new List<object>() { ItemName, Amount };
        }
    }
}
