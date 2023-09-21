namespace FeloxGame
{
    public class Tile
    {
        public string Name { get; private set; }
        public string TextureLocation { get; private set; }
        public int TextureIndex { get; private set; }
        public bool IsCollidable { get; private set; } = false;

        public Tile(string name, string textureLocation, int textureIndex, bool isCollidable)
        {
            Name = name;
            TextureLocation = textureLocation;
            TextureIndex = textureIndex;
            IsCollidable = isCollidable;
        }

    }
}
