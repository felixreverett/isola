using FeloxGame.World;

namespace FeloxGame.Saving
{
    /// <summary>
    /// Serializable save data for the chunk and its entities
    /// </summary>
    public class ChunkSaveData
    {
        public string ChunkID { get; set; }
        public int ChunkPosX { get; set; }
        public int ChunkPosY { get; set; }
        public List<object> EntitySaveDataList { get; set; }
        public ChunkTile[] ChunkTiles { get; set; }
    }
}
