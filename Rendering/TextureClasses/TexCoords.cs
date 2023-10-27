namespace FeloxGame.Rendering
{
    public class TexCoords
    {
        public float MinX { get; set; } = -1.0f;
        public float MinY { get; set; } = -1.0f;
        public float MaxX { get; set; } = 1.0f;
        public float MaxY { get; set; } = 1.0f;

        public TexCoords() { }

        public TexCoords(float minX, float minY, float maxX, float maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }
    }
}
