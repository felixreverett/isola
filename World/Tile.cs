namespace FeloxGame
{
    public class Tile
    {
        public string Name { get; set; }
        public string TextureLocation { get; set; }

        public Tile(string name, string textureLocation)
        {
            Name = name;
            TextureLocation = textureLocation;
        }

    }
}
