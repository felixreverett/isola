namespace Isola.Drawing
{
    public class IndexedTextureAtlasManager : IAtlasManager
    {
        // "Indexed Atlas" Properties
        public string AtlasFileName { get; private set; }
        public int AtlasSize { get; private set; }
        protected float Offset { get; private set; }
        protected bool UseOffset { get; private set; }
        public int TextureSize { get; private set; }
        public int Padding { get; private set; }
        public int RowColumnLength { get; private set; }
        public int MaxIndex {  get; private set; }

        public IndexedTextureAtlasManager(string atlasFileName, int atlasSize,
            int textureSize, int padding, bool useOffset = false, float zDepthLayer = 0.0f)
        {
            AtlasFileName = atlasFileName;
            AtlasSize = atlasSize;
            Offset = 2.0f / (AtlasSize * 2);
            UseOffset = useOffset;
            TextureSize = textureSize;
            Padding = padding;
            RowColumnLength = AtlasSize / (TextureSize + Padding);
            MaxIndex = RowColumnLength * RowColumnLength - 1;
        }

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
