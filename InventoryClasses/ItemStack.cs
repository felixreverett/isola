namespace FeloxGame.InventoryClasses
{
    public class ItemStack
    {
        public string ItemName { get; set; }
        public int Amount { get; set; }
        public ItemStack(string itemName, int amount)
        {
            this.ItemName = itemName;
            this.Amount = amount;
        }

        public virtual void Use()
        {
            //
        }
    }
}
