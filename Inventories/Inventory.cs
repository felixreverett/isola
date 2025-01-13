using FeloxGame.Inventories;

namespace FeloxGame
{
    // Inventory items will only be accessed through methods like .Add() and .Remove()
    // The basic inventory class for universal inventory functionality
    // Should be an inventory "instance" which is only instantiated/loaded when accessed by the player, and is saved when closed (05/01/25)
    public class Inventory
    {
        public ItemStack[] ItemStackList { get; set; }
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public Inventory(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            ItemStackList = new ItemStack[rows * cols];
        }

        public void AddItemStack(ItemStack itemStack)
        {
            ItemStack matchingItemStack = ItemStackList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);
            
            if (matchingItemStack.Equals(default(ItemStack)))
            {
                if (GetFirstFreeIndex(out var index))
                {
                    ItemStackList[index] = itemStack;
                }
            }
            else
            {
                matchingItemStack.Amount += itemStack.Amount;
            }
        }

        public void RemoveItemStack(ItemStack itemStack)
        {
            ItemStack matchingItemStack = ItemStackList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);

            if (matchingItemStack.Equals(default(ItemStack)))
            {
                throw new ArgumentException("Error. Attempted to remove an item not in the inventory");
            }

            else if (matchingItemStack.Amount > itemStack.Amount)
            {
                matchingItemStack.Amount -= itemStack.Amount;
            }

            else if (matchingItemStack.Amount == itemStack.Amount)
            {
                int index = Array.IndexOf(ItemStackList, matchingItemStack);
                ItemStackList[index] = default(ItemStack);
            }

            else if (matchingItemStack.Amount < itemStack.Amount)
            {
                throw new ArgumentException("Error. Tried to remove more items than in inventory");
            }
        }

        public void AddToSlotIndex(ItemStack itemStack, int slotIndex)
        {
            if (ItemStackList[slotIndex].Equals(default(ItemStack)))
            {
                ItemStackList[slotIndex] = itemStack;
            }

            else
            {
                // do nothing
            }
        }

        public bool GetFirstFreeIndex(out int index)
        {
            index = -1;
            for (int i = 0; i < ItemStackList.Length; i++)
            {
                if (ItemStackList[i].Equals(default(ItemStack)))
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }

        public void Sort(eSortType sortType = eSortType.Alphabetical)
        {
            ItemStack[] sortedItems;
            switch (sortType)
            {
                case eSortType.Alphabetical:
                    sortedItems = ItemStackList.OrderBy(i => i.ItemName).ToArray();
                    ItemStackList = sortedItems;
                    break;
                case eSortType.Amount:
                    sortedItems = ItemStackList.OrderBy(i => i.Amount).ThenBy(i => i.ItemName).ToArray();
                    break;
                case eSortType.Category:
                    // not yet implemented
                    break;
            }
        }
    }
}
