namespace Isola.Drawing
{
    public class PrecisionTextureAtlasManager : IAtlasManager
    {
        public string AtlasFileName { get; private set; }
        public int AtlasSize { get; private set; }
        private int AtlasWidth { get; set; }
        private int AtlasHeight { get; set; }

        protected float Offset { get; private set; }
        protected bool UseOffset { get; private set; }

        public PrecisionTextureAtlasManager(string atlasFileName, int atlasSize,
            int atlasWidth, int atlasHeight, bool useOffset = false, float zDepthLayer = 0.0f)
        {
            AtlasWidth = atlasWidth;
            AtlasHeight = atlasHeight;
            AtlasFileName = atlasFileName;
            AtlasSize = atlasSize;
            Offset = 2.0f / (AtlasSize * 2);
            UseOffset = useOffset;
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
