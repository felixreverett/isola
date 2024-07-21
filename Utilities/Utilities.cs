using FeloxGame.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace FeloxGame.Utilities
{
    public class Utilities
    {
        public static int GetSizeOfVertexAttribPointerType(VertexAttribPointerType attribType)
        {
            switch (attribType)
            {
                case VertexAttribPointerType.UnsignedByte:
                    return 1;
                case VertexAttribPointerType.UnsignedInt:
                    return 4;
                case VertexAttribPointerType.Float:
                    return 4;
                default:
                    return 0;
            }
        }

        // Todo: remove once ItemEntity stop needing it
        /// <summary>
        /// Returns Atlas coordinates for a texture at the given index. Use offset for tiles.
        /// </summary>
        /// <returns></returns>
        public static TexCoords GetIndexedAtlasCoords(int textureIndex, int textureSize, int atlasSize, int padding, bool useOffset = false)
        {
            TexCoords texCoords = new TexCoords();

            int rowColumnLength = atlasSize / (textureSize + padding);
            int column = textureIndex % rowColumnLength;
            int row = textureIndex / rowColumnLength;
            float offset = 0.2f / (atlasSize * 2);
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
    }
}