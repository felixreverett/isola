using FeloxGame.Utilities;
using FeloxGame.Rendering;
using FeloxGame.Inventories;
using FeloxGame.Items;
using OpenTK.Mathematics;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FeloxGame.Entities
{
    public class ItemEntity : Entity
    {
        public new eEntityType EntityType = eEntityType.ItemEntity;
        public string ItemName { get; set; }
        public int Amount { get; set; }

        // Initialize entity otherwise
        public ItemEntity(eEntityType entityType, Vector2 position, string itemName, int amount, string textureAtlasName = "Item Atlas") 
            : base(entityType, position, textureAtlasName)
        {
            this.ItemName = itemName;
            this.Amount = amount;
            SetItemTexture();
        }

        // Initialize Entity from save data
        public ItemEntity(ItemEntitySaveData saveData)
            : base(saveData)
        {
            this.ItemName = saveData.ItemName;
            this.Amount = saveData.Amount;
            SetItemTexture();
        }
        
        private void SetItemTexture()
        {
            Item matchingItem = AssetLibrary.ItemList.FirstOrDefault(i => i.ItemName == ItemName)!;
            int index = matchingItem == null ? 0 : matchingItem.TextureIndex;
            
            TexCoords texCoords = Utilities.Utilities.GetIndexedAtlasCoords(index, 16, 1024, 8);
            
            // Set texCoords of atlas
            vertices[3] =  texCoords.MaxX; vertices[4] =  texCoords.MaxY; // (1, 1)
            vertices[11] = texCoords.MaxX; vertices[12] = texCoords.MinY; // (1, 0)
            vertices[19] = texCoords.MinX; vertices[20] = texCoords.MinY; // (0, 0)
            vertices[27] = texCoords.MinX; vertices[28] = texCoords.MaxY; // (0, 1)
        }

        // Export entity save data
        public override EntitySaveDataObject GetSaveData()
        {
            ItemEntitySaveData data = new
                (
                    new float[] { Position.X, Position.Y },     // 0
                    new float[] { Size.X, Size.Y },             // 1
                    ItemName,                                   // 2
                    Amount                                      // 3
                );

            return new EntitySaveDataObject(EntityType, JsonSerializer.Serialize(data));
        }
    }
}
