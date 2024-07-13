using FeloxGame.Utilities;
using System.Text.Json.Serialization;

namespace FeloxGame.World
{
    public class ChunkTile
    {
        public int TileID { get; set; }
        public Dictionary<string, object> Metadata { get; private set; }
        public ChunkTile(int tileID)
        {
            TileID = tileID;
            Metadata = new Dictionary<string, object>();
        }

        [JsonConstructor]
        public ChunkTile(int tileID, Dictionary<string, object> metadata)
        {
            TileID = tileID;
            Metadata = metadata;
        }

        /// <summary>
        /// Gets the tile name for the tile from the AssetLibrary
        /// </summary>
        /// <returns></returns>
        public string GetTileName()
        {
            return AssetLibrary.TileList
                .Where(tile => tile.TileID == this.TileID)
                .FirstOrDefault().Name;
        }
    }
}
