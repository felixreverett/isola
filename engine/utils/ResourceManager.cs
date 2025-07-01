﻿using FeloxGame.Drawing;

namespace FeloxGame.Utilities
{
    //Todo: consider removing class if unused? (only used by TextureAtlas)
    public class ResourceManager
    {
        private static ResourceManager _instance = null;
        private static readonly object _loc = new();
        private IDictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>(); // Dictionary for texture cache

        public static ResourceManager Instance
        {
            get
            {
                lock(_loc)
                {
                    if (_instance == null)
                    {
                        _instance = new ResourceManager();
                    }
                    return _instance;
                }
            }
        }

        public Texture2D LoadTextureAtlas(string atlasFileName, int textureUnit)
        {
            string texturePath = @"../../../Resources/TextureAtlases/" + atlasFileName;
            _textureCache.TryGetValue(texturePath, out var value);
            if (value is not null)
            {
                return value;
            }
            value = TextureFactory.Load(texturePath, textureUnit);
            _textureCache.Add(texturePath, value);
            return value;
        }
    }
}
