using FeloxGame.Core.Management;

namespace FeloxGame.Core.Management
{
    public class TextureManager
    {
        private static TextureManager _instance = null;
        private static readonly object _loc = new();
                
        public static TextureManager Instance
        {
            get
            {
                lock (_loc)
                {
                    if (_instance == null)
                    {
                        _instance = new TextureManager();
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Returns Atlas coordinates for a texture at the given index. Use offset for tiles.
        /// </summary>
        /// <returns></returns>
        public TexCoords GetIndexedAtlasCoords(int textureIndex, int textureSize, int atlasSize, int padding, bool useOffset = false)
        {
            TexCoords texCoords = new TexCoords();

            int rowColumnLength = atlasSize / (textureSize + padding);
            int column = textureIndex % rowColumnLength;
            int row = textureIndex / rowColumnLength;
            float offset = 1f / (atlasSize * 2);
            float normalisedOffset = (textureSize + padding) / (float)atlasSize;
            float normalisedTextureSize = textureSize / (float)atlasSize;
            
            texCoords.MinX = (float)column * normalisedOffset;
            texCoords.MaxX = texCoords.MinX + normalisedTextureSize;
            texCoords.MinY = (float)row * normalisedOffset;
            texCoords.MaxY = texCoords.MinY + normalisedTextureSize;

            if (useOffset)
            {
                texCoords.MinX += offset;
                texCoords.MaxX -= offset;
                texCoords.MinY += offset;
                texCoords.MaxY -= offset;
            }

            return texCoords;
        }

        public TexCoords GetPrecisionAtlasCoords(float x, float y, float textureWidth, float textureHeight, float atlasWidth, float atlasHeight)
        {
            //todo: add bounds error checking?
            TexCoords texCoords = new TexCoords();
            float pixelOffsetX = 1f / (atlasWidth * 2);
            float pixelOffsetY = 1f / (atlasHeight * 2);
            texCoords.MinX = x / atlasWidth;
            texCoords.MinY = y / atlasHeight;
            texCoords.MaxX = (x + textureWidth) / atlasWidth;
            texCoords.MaxY = (y + textureHeight) / atlasHeight;

            return texCoords;
        }
    }
}
