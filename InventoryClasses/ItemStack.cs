namespace FeloxGame.InventoryClasses
{
    public class ItemStack
    {
        public Item Item { get; set; }
        public int Amount { get; set; }
        public ItemStack()
        {
            this.Item = new Item();
        }
    }
}
