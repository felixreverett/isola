using FeloxGame.Core.Management;
using FeloxGame.Core;
using FeloxGame.InventoryClasses;
using OpenTK.Mathematics;

namespace FeloxGame.EntityClasses
{
    public class ItemEntity : Entity
    {
        public ItemStack ItemStack { get; set; }

        public ItemEntity(Vector2 position, Vector2 size, ItemStack itemStack, string textureAtlasName = "Items/Item Atlas.png", int textureUnit = 3) 
            : base(position, size, textureAtlasName, textureUnit)
        {
            UpdateItem(itemStack);
        }

        public void OnLoad()
        {

        }

        private void UpdateItem(ItemStack itemStack)
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

        private void SetTextureCoords(TexCoords texCoords)
        {
            // Set texCoords of atlas
            vertices[3] =  texCoords.MaxX; vertices[4] =  texCoords.MaxY; // (1, 1)
            vertices[11] = texCoords.MaxX; vertices[12] = texCoords.MinY; // (1, 0)
            vertices[19] = texCoords.MinX; vertices[20] = texCoords.MinY; // (0, 0)
            vertices[27] = texCoords.MinX; vertices[28] = texCoords.MaxY; // (0, 1)
        }
    }
}
