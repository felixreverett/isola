using FeloxGame.Core;
using FeloxGame.Core.Management;
using FeloxGame.InventoryClasses;

namespace FeloxGame.GUI
{
    public class ItemSlotUI : UI
    {
        int ItemSlotID;
        Inventory Inventory;

        public ItemSlotUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool drawable, bool toggleDraw, bool isClickable,
            TexCoords koPosition, int itemSlotID, Inventory inventory
        ) 
            : base(koWidth, koHeight, anchor, scale, drawable, toggleDraw, isClickable)
        {
            this.KoPosition = koPosition;
            this.ItemSlotID = itemSlotID;
            this.Inventory = inventory;
            inventoryAtlas = ResourceManager.Instance.LoadTexture("Items/Item Atlas.png", 3);
        }

        /// <summary>
        /// Updates information about the item at this item slot. 
        /// </summary>
        public void UpdateItem(ItemStack itemStack)
        {
            int index = 0;
            
            Item matchingItem = AssetLibrary.ItemList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);
            
            if (matchingItem != null)
            {
                index = matchingItem.TextureIndex;
            }
            
            TexCoords texCoords = TextureManager.Instance.GetIndexedAtlasCoords(index, 32, 1024, 8);

            SetTextureCoords(texCoords);
        }

        public void SetTextureCoords(TexCoords texCoords)
        {
            // Set texCoords of atlas
            Vertices[3] = texCoords.MaxX; Vertices[4] = texCoords.MaxY; // (1, 1)
            Vertices[11] = texCoords.MaxX; Vertices[12] = texCoords.MinY; // (1, 0)
            Vertices[19] = texCoords.MinX; Vertices[20] = texCoords.MinY; // (0, 0)
            Vertices[27] = texCoords.MinX; Vertices[28] = texCoords.MaxY; // (0, 1)
        }

        public override void OnClick()
        {
            base.OnClick();
            Inventory.OnItemSlotClick(ItemSlotID);
        }

    }
}
