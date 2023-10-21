using FeloxGame.Core.Management;

namespace FeloxGame.Core.Rendering
{
    public class TextureAtlas
    {
        public int AtlasSize { get; private set; }
        public int TextureSize { get; private set; }
        public int Padding { get; private set; }
        public int MaxIndex { get; private set; }
        public Texture2D Atlas { get; private set; }

        public TextureAtlas(int atlasSize, int textureSize, int padding, string atlasName, int textureUnit)
        {
            AtlasSize = atlasSize;
            TextureSize = textureSize;
            Padding = padding;

            MaxIndex = ((int)atlasSize / (textureSize + padding)) ^ 2 - 1;

            Atlas = ResourceManager.Instance.LoadTexture(atlasName, textureUnit);
        }


        // Todo: add optimisations once working
        public TexCoords GetIndexedAtlasCoords(int textureIndex, bool useOffset = false)
        {
            if (textureIndex > MaxIndex) 
            {
                textureIndex = 0;
            }

            TexCoords texCoords = new TexCoords();

            int rowColumnLength = AtlasSize / (TextureSize + Padding);
            int column = textureIndex % rowColumnLength;
            int row = textureIndex / rowColumnLength;
            float offset = 0.2f / (AtlasSize * 2);
            float normalisedOffset = (TextureSize + Padding) / (float)AtlasSize;
            float normalisedTextureSize = TextureSize / (float)AtlasSize;

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
