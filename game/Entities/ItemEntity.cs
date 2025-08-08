using Isola.Drawing;
using Isola.Utilities;
using OpenTK.Mathematics;
using System.Text.Json;

namespace Isola.Entities
{
    public class ItemEntity : Entity
    {
        public string ItemName { get; set; }
        public int Amount { get; set; }
        private IndexedTextureAtlasManager AtlasManager { get; set; }

        // Initialize entity otherwise
        public ItemEntity(Vector2 position, string itemName, int amount) 
            : base(position)
        {
            this.ItemName = itemName;
            this.Amount = amount;
            AtlasManager = (IndexedTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Item Atlas"];
            BatchRenderer = AssetLibrary.BatchRendererList["Item Atlas"];
            SetTexCoords();
        }

        // Initialize Entity from save data
        public ItemEntity(ItemEntitySaveData saveData)
            : base(saveData)
        {
            this.ItemName = saveData.ItemName;
            this.Amount = saveData.Amount;
            AtlasManager = (IndexedTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Item Atlas"];
            BatchRenderer = AssetLibrary.BatchRendererList["Item Atlas"];
            SetTexCoords();
        }
        
        private void SetTexCoords()
        {
            Items.Item? matchingItem = AssetLibrary.ItemList!.FirstOrDefault(i => i.ItemName == ItemName)!;
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

            return new EntitySaveDataObject(eEntityType.ItemEntity, JsonSerializer.Serialize(data));
        }
    }
}
