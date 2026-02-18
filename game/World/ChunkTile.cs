using Isola.Utilities;

namespace Isola.World {
    public struct ChunkTile {
        public int TileID { get; set; }
        public ChunkTile(int tileID) {
            TileID = tileID;
        }

        public string GetTileName(AssetLibrary assets) {
            int id = TileID;
            return assets.TileList.FirstOrDefault(t => t.TileID == id).Name;
        }
    }
}
