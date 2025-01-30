using OpenTK.Mathematics;
using System.Text.Json;

namespace FeloxGame.Entities
{
    public class TileEntity_Chest : TileEntity
    {
        public Inventory Inventory { get; set; }
        public TileEntity_Chest(eEntityType entityType, Vector2 position, Vector2 drawPositionOffset)
            : base(entityType, position, drawPositionOffset)
        {
            Inventory = new Inventory(5, 10);
        }

        public TileEntity_Chest(TileEntity_Chest_SaveData saveData)
            : base(saveData)
        {
            Inventory = saveData.Inventory;
        }

        public override EntitySaveDataObject GetSaveData()
        {
            TileEntity_Chest_SaveData data = new
                (
                    new float[] { Position.X, Position.Y },                     // 0
                    new float[] { Size.X, Size.Y },                             // 1
                    new float[] { DrawPositionOffset.X, DrawPositionOffset.Y }, // 2
                    Inventory                                                   // 3
                );

            return new EntitySaveDataObject(EntityType, JsonSerializer.Serialize(data));
        }
    }
}
