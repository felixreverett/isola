using FeloxGame.Utilities;
using FeloxGame.Items;
using OpenTK.Mathematics;
using System.Text.Json;

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
            SetTexCoords();
        }

        // Initialize Entity from save data
        public ItemEntity(ItemEntitySaveData saveData)
            : base(saveData)
        {
            this.ItemName = saveData.ItemName;
            this.Amount = saveData.Amount;
            SetTexCoords();
        }
        
        private void SetTexCoords()
        {
            Item? matchingItem = AssetLibrary.ItemList!.FirstOrDefault(i => i.ItemName == ItemName)!;
            int index = matchingItem == null ? 0 : matchingItem.TextureIndex;
            TexCoords = Utilities.Utilities.GetIndexedAtlasCoords(index, 16, 1024, 8);
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
