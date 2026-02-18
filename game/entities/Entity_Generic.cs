using Isola.Drawing;
using Isola.Entities;
using Isola.Utilities;
using OpenTK.Mathematics;

namespace Isola.game.entities {
    public class Entity_Generic : Entity {
        private IndexedTextureAtlasManager AtlasManager { get; set; }
        public Entity_Generic(Vector2 position, AssetLibrary assets)
            : base(position, assets) {
            AtlasManager = (IndexedTextureAtlasManager)_assets.TextureAtlasManagerList["Item Atlas"];
            BatchRenderer = _assets.BatchRendererList["Item Atlas"];
        }
        
        public Entity_Generic(EntitySaveData saveData, AssetLibrary assets)
            : base(saveData, assets) {
            AtlasManager = (IndexedTextureAtlasManager)_assets.TextureAtlasManagerList["Item Atlas"];
            BatchRenderer = _assets.BatchRendererList["Item Atlas"];
        }
    }
}
