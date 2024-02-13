using FeloxGame.UtilityClasses;
using FeloxGame.Rendering;
using FeloxGame.InventoryClasses;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace FeloxGame.EntityClasses
{
    public class ItemEntity : Entity
    {
        [JsonInclude]
        public ItemStack ItemStack { get; set; }

        public ItemEntity(Vector2 position, ItemStack itemStack, string textureAtlasName = "Item Atlas") 
            : base(position, textureAtlasName)
        {
            UpdateItem(itemStack);
        }

        private void UpdateItem(ItemStack itemStack)
        {
            int index = 0;

            Item matchingItem = AssetLibrary.ItemList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);

            if (matchingItem != null)
            {
                index = matchingItem.TextureIndex;
            }

            TexCoords texCoords = Utilities.GetIndexedAtlasCoords(index, 16, 1024, 8);

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

        /*public override ItemEntitySaveData SaveData()
        {
            return new ItemEntitySaveData(Position, Size, ItemStack);
        }*/

        public override void LoadData(EntitySaveData entitySaveData)
        {
            base.LoadData(entitySaveData);
        }
    }
}
