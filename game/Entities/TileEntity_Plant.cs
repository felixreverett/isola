using Isola.engine.graphics;
using Isola.Entities;
using Isola.Utilities;
using OpenTK.Mathematics;
using System.Text.Json;

namespace Isola{
    public class TileEntity_Plant : TileEntity {
        public int MaximumGrowthStages { get; set; } = 1; //todo: remove this and shift over to EntityStaticData
        public int GrowthStage { get; set; } = 1;
        public EntityTextureAtlasManager AtlasManager { get; set; }

        // Initialize TileEntity from save data
        public TileEntity_Plant(TileEntity_Plant_SaveData saveData, AssetLibrary assets)
            : base(saveData, assets) {
            AtlasManager = (EntityTextureAtlasManager)_assets.TextureAtlasManagerList["Entity Atlas"];
            BatchRenderer = _assets.BatchRendererList["Entity Atlas"];
            SetTexCoords();
        }

        // Default constructor
        public TileEntity_Plant(Vector2 position, Vector2 drawPositionOffset, AssetLibrary assets, int growthStage = 0)
            : base(position, drawPositionOffset, assets) {
            GrowthStage = growthStage;
            AtlasManager = (EntityTextureAtlasManager)_assets.TextureAtlasManagerList["Entity Atlas"];
            BatchRenderer = _assets.BatchRendererList["Entity Atlas"];
            SetTexCoords();
        }

        private void SetTexCoords() {
            TexCoords = AtlasManager.GetAtlasCoords("Wood Chest"); //todo: update per chest
        }

        // Export entity save data
        public override EntitySaveDataObject GetSaveData() {
            TileEntity_Plant_SaveData data = new (
                new float[] { Position.X, Position.Y },                     // 0
                new float[] { Size.X, Size.Y },                             // 1
                new float[] { DrawPositionOffset.X, DrawPositionOffset.Y }, // 2
                GrowthStage                                                 // 3
            );

            return new EntitySaveDataObject(eEntityType.TileEntity_Plant, JsonSerializer.Serialize(data));
        }
    }
}
