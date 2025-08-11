using Isola.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Isola.Drawing
{
    /// <summary>
    /// Handles information pertaining to a texture unit
    /// Compare "BatchRenderer" classes, which are solely responsible for the sampling of and drawing from textureUnits
    /// </summary>
    public class TextureAtlasManager //todo Aug 2025: remove this if fully obsolete
    {
        // Atlas information
        public string AtlasFileName { get; private set; }
        public int AtlasSize { get; private set; }
        protected float Offset { get; private set; }
        protected bool UseOffset { get; private set; }
        
        public TextureAtlasManager(int textureUnit, string atlasFileName, int atlasSize,
            bool useOffset = false, float zDepthLayer = 0.0f)
        {
            AtlasFileName = atlasFileName;
            AtlasSize = atlasSize;
            Offset = 2.0f / (AtlasSize * 2);
            UseOffset = useOffset;
        }
    }
}
