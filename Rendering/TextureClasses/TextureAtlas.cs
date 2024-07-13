using FeloxGame.Utilities;

namespace FeloxGame.Rendering
{
    public class TextureAtlas
    {
        public int AtlasSize { get; private set; }
        protected float Offset { get; private set; }
        protected bool UseOffset { get; private set; }
        public Texture2D Texture { get; private set; }

        public TextureAtlas(int atlasSize, string atlasName, int textureUnit, bool useOffset = false)
        {
            AtlasSize = atlasSize;
                        
            Offset = 0.2f / (AtlasSize * 2);
            UseOffset = useOffset;

            Texture = ResourceManager.Instance.LoadTexture(atlasName, textureUnit);
        }
    }
}
