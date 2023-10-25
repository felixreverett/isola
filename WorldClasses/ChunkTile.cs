using System.Text.Json.Serialization;

namespace FeloxGame.WorldClasses
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
    }
}
