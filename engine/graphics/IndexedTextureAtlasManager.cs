namespace Isola.Drawing
{
    public class IndexedTextureAtlasManager : TextureAtlasManager
    {
        // "Indexed Atlas" Properties
        public int TextureSize { get; private set; }
        public int Padding { get; private set; }
        public int RowColumnLength { get; private set; }
        public int MaxIndex {  get; private set; }
        
        public IndexedTextureAtlasManager(int textureUnit, string atlasFileName, int atlasSize,
            int textureSize, int padding, bool useOffset = false, float zDepthLayer = 0.0f)
            : base(textureUnit, atlasFileName, atlasSize, useOffset, zDepthLayer)
        {
            TextureSize = textureSize;
            Padding = padding;
            RowColumnLength = AtlasSize / (TextureSize + Padding);
            MaxIndex = RowColumnLength * RowColumnLength - 1;
        }

        public override TexCoords GetIndexedAtlasCoords(int textureIndex)
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

            texCoords.MinX = column * normalisedOffset;
            texCoords.MaxX = texCoords.MinX + normalisedTextureSize;
            texCoords.MinY = row * normalisedOffset;
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
