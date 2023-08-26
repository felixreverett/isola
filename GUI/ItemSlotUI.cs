using FeloxGame.Core;
using FeloxGame.Core.Management;
using FeloxGame.InventoryClasses;
using FeloxGame.WorldClasses;

namespace FeloxGame.GUI
{
    public class ItemSlotUI : UI
    {
        public ItemSlotUI(float koWidth, float koHeight, eAnchor anchor, float scale, bool drawable, bool toggleDraw, TexCoords koPosition) 
            : base(koWidth, koHeight, anchor, scale, drawable, toggleDraw)
        {
            this.KoPosition = koPosition;
            inventoryAtlas = ResourceManager.Instance.LoadTexture("Items/Item Atlas.png", 3);

            SetTextureCoords(4, 840, 346, 180, 1024, 1024);
        }

        /// <summary>
        /// Updates information about the item at this item slot. 
        /// </summary>
        public void UpdateItem(ItemStack itemStack)
        {
            int index = 0;
            Item matchingItem = ThingWhereAllTheItemsAreStored.ItemList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);
            if (matchingItem != null)
            {
                index = matchingItem.TextureIndex;
            }
            
            TexCoords texCoords = WorldManager.Instance.GetIndexedAtlasCoords(index, 32, 1024, 8);

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

    }
}
