namespace FeloxGame.Drawing
{
    public class PrecisionTextureAtlasManager : TextureAtlasManager
    {
        private int AtlasWidth { get; set; }
        private int AtlasHeight { get; set; }

        public PrecisionTextureAtlasManager(int textureUnit, string atlasFileName, int atlasSize,
            int atlasWidth, int atlasHeight, bool useOffset = false, float zDepthLayer = 0.0f)
            : base(textureUnit, atlasFileName, atlasSize, useOffset, zDepthLayer)
        {
            AtlasWidth = atlasWidth;
            AtlasHeight = atlasHeight;
        }

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
