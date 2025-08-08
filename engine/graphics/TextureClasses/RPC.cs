namespace Isola.Drawing
{
    public class RPC
    {
        public float MinX { get; set; } = -1.0f;
        public float MinY { get; set; } = -1.0f;
        public float MaxX { get; set; } = 1.0f;
        public float MaxY { get; set; } = 1.0f;

        public RPC() { }

        public RPC(float minX, float minY, float maxX, float maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }
    }
}
