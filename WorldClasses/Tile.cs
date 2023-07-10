namespace FeloxGame
{
    public class Tile
    {
        public string Name { get; private set; }
        public string TextureLocation { get; private set; }
        public int TextureIndex { get; private set; }

        public Tile(string name, string textureLocation, int textureIndex)
        {
            Name = name;
            TextureLocation = textureLocation;
            TextureIndex = textureIndex;
        }

    }
}
