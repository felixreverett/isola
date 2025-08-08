using Isola.Entities;
using Isola.Inventories;

namespace Isola
{
    // The Player's Inventory
    public class PlayerInventory : Inventory
    {
        // only the player inventory has an additional mouseslot
        public ItemStack MouseSlotItemStack { get; set; }

        public PlayerInventory(int rows, int cols)
            : base(rows, cols) {}

    }
}
