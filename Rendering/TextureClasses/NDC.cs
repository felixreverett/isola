namespace FeloxGame.Rendering
{
    public class NDC
    {
        public float MinX { get; set; } = -1.0f;
        public float MinY { get; set; } = -1.0f;
        public float MaxX { get; set; } = 1.0f;
        public float MaxY { get; set; } = 1.0f;

        public NDC() { }

        public NDC(float minX, float minY, float maxX, float maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }
    }
}
