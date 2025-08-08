using Isola.Drawing;
using Isola.Entities;
using Isola.Utilities;
using OpenTK.Mathematics;

namespace Isola.game.entities
{
    public class Entity_Generic : Entity
    {
        private IndexedTextureAtlasManager AtlasManager { get; set; }
        public Entity_Generic(Vector2 position)
            : base(position)
        {
            AtlasManager = (IndexedTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Item Atlas"];
            BatchRenderer = AssetLibrary.BatchRendererList["Item Atlas"];
        }
        
        public Entity_Generic(EntitySaveData saveData)
            : base(saveData) 
        {
            AtlasManager = (IndexedTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Item Atlas"];
            BatchRenderer = AssetLibrary.BatchRendererList["Item Atlas"];
        }
    }
}
