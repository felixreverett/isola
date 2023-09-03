using FeloxGame.InventoryClasses;

namespace FeloxGame
{
    // Inventory items will only be accessed through methods like .Add() and .Remove()
    public class Inventory
    {
        public ItemStack[] _itemStackList;
        public ItemStack _mouseSlotItemStack;
        private int _rows;
        private int _cols;

        public Inventory(int rows, int cols)
        {
            this._rows = rows;
            this._cols = cols;
            this._itemStackList = new ItemStack[rows*cols];
        }

        public event Action<ItemStack[]> InventoryChanged;

        public void Add(ItemStack itemStack)
        {
            var matchingItemStack = _itemStackList.FirstOrDefault(i => i is not null && i.ItemName == itemStack.ItemName);
            
            if (matchingItemStack is null)
            {
                if (FirstFreeIndex(out var index))
                {
                    _itemStackList[index] = itemStack;
                }
            }
            else
            {
                matchingItemStack.Amount += itemStack.Amount;
            }

            InventoryChanged?.Invoke(_itemStackList);
        }

        public void Remove(ItemStack itemStack)
        {
            var matchingItemStack = _itemStackList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);

            if ( matchingItemStack is null)
            {
                throw new ArgumentException("Error. Attempted to remove an item not in the inventory");
            }
            else if (matchingItemStack.Amount > itemStack.Amount)
            {
                matchingItemStack.Amount -= itemStack.Amount;
            }
            else if (matchingItemStack.Amount == itemStack.Amount)
            {
                int index = Array.IndexOf(_itemStackList, matchingItemStack);
                _itemStackList[index] = null;
            }
            else if (matchingItemStack.Amount < itemStack.Amount)
            {
                throw new ArgumentException("Error. Tried to remove more items than in inventory");
            }

            InventoryChanged?.Invoke(_itemStackList);
        }

        public bool FirstFreeIndex(out int index)
        {
            index = -1;
            for (int i = 0; i < _itemStackList.Length; i++)
            {
                if (_itemStackList[i] is null)
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
                    sortedItems = _itemStackList.OrderBy(i => i.ItemName).ToArray();
                    _itemStackList = sortedItems;
                    break;
                case eSortType.Amount:
                    sortedItems = _itemStackList.OrderBy(i => i.Amount).ThenBy(i => i.ItemName).ToArray();
                    break;
                case eSortType.Category:
                    // not yet implemented
                    break;
            }

            InventoryChanged?.Invoke(_itemStackList);
        }

        public void OnItemSlotClick(int slotIndex)
        {
            Console.WriteLine($"Slot {slotIndex} was clicked!");
        }
    }
}
