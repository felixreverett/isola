using OpenTK.Mathematics;
using FeloxGame.Entities;
using System.Text.Json;

namespace FeloxGame
{
    public class TileEntity_Plant : TileEntity
    {
        public int MaximumGrowthStages { get; set; } = 1; //todo: remove this and shift over to EntityStaticData
        public int GrowthStage { get; set; } = 1;

        // Initialize TileEntity from save data
        public TileEntity_Plant(TileEntity_Plant_SaveData saveData)
            : base(saveData)
        {
        }

        // Default constructor
        public TileEntity_Plant(eEntityType entityType, Vector2 position, Vector2 drawPositionOffset, int growthStage = 0)
            : base(entityType, position, drawPositionOffset)
        {
            GrowthStage = growthStage;
        }

        // Export entity save data
        public override EntitySaveDataObject GetSaveData()
        {
            TileEntity_Plant_SaveData data = new
                (
                    new float[] { Position.X, Position.Y },                     // 0
                    new float[] { Size.X, Size.Y },                             // 1
                    new float[] { DrawPositionOffset.X, DrawPositionOffset.Y }, // 2
                    GrowthStage                                                 // 3
                );

            return new EntitySaveDataObject(EntityType, JsonSerializer.Serialize(data));
        }
    }
}
