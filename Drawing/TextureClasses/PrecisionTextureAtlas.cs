using FeloxGame.Core.Management;

namespace FeloxGame.Drawing
{
    public class PrecisionTextureAtlas : TextureAtlas
    {
        public int AtlasWidth { get; private set; }
        public int AtlasHeight { get; private set; }

        public PrecisionTextureAtlas(int atlasSize, string atlasName, int textureUnit, int atlasWidth, int atlasHeight, bool useOffset = false)
            : base(atlasSize, atlasName, textureUnit, useOffset)
        {
            AtlasWidth = atlasWidth;
            AtlasHeight = atlasHeight;
        }

        /// <summary>
        /// Returns the relative TexCoords starting at (x,y) to the dimensions of the texture
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="textureWidth"></param>
        /// <param name="textureHeight"></param>
        /// <returns></returns>
        public TexCoords GetPrecisionAtlasCoords(float x, float y, float textureWidth, float textureHeight)
        {
            //todo: add bounds error checking?
            TexCoords texCoords = new TexCoords();
            
            texCoords.MinX = x / AtlasWidth;
            texCoords.MinY = y / AtlasHeight;
            texCoords.MaxX = (x + textureWidth) / AtlasWidth;
            texCoords.MaxY = (y + textureHeight) / AtlasHeight;

            return texCoords;
        }

    }
}
