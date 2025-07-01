using FeloxGame.Entities;
using FeloxGame.Inventories;

namespace FeloxGame
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
