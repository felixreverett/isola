namespace FeloxGame.Rendering
{
    public class IndexedTextureAtlas : TextureAtlas
    {
        public int TextureSize { get; private set; }
        public int Padding { get; private set; }
        public int RowColumnLength { get; private set; }
        public int MaxIndex { get; private set; }

        public IndexedTextureAtlas(int atlasSize, int textureSize, int padding, string atlasName, int textureUnit, bool useOffset = false)
            : base(atlasSize, atlasName, textureUnit, useOffset)
        {
            TextureSize = textureSize;
            Padding = padding;

            RowColumnLength = AtlasSize / (TextureSize + Padding);
            MaxIndex = RowColumnLength * RowColumnLength - 1;
        }

        // Todo: add optimisations once working
        public TexCoords GetIndexedAtlasCoords(int textureIndex)
        {
            if (textureIndex > MaxIndex)
            {
                textureIndex = 0;
            }

            TexCoords texCoords = new TexCoords();

            int column = textureIndex % RowColumnLength;
            int row = textureIndex / RowColumnLength;

            float normalisedOffset = (TextureSize + Padding) / (float)AtlasSize;
            float normalisedTextureSize = TextureSize / (float)AtlasSize;

            texCoords.MinX = (float)column * normalisedOffset;
            texCoords.MaxX = texCoords.MinX + normalisedTextureSize;
            texCoords.MinY = (float)row * normalisedOffset;
            texCoords.MaxY = texCoords.MinY + normalisedTextureSize;

            if (UseOffset)
            {
                texCoords.MinX += Offset;
                texCoords.MaxX -= Offset;
                texCoords.MinY += Offset;
                texCoords.MaxY -= Offset;
            }

            return texCoords;
        }
    }
}
