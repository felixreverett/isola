using OpenTK.Mathematics;

namespace FeloxGame.GameClasses
{
    public class GameCursor
    {
        public Vector2 WorldPosition { get; set; }
        public Vector2 ScreenPosition { get; set; }

        public int Rounded(float f)
        {
            return (int)Math.Floor(f);
        }

        public void UpdateWorldAndScreenPosition(Vector2 mousePosition, Vector3 cameraPosition, Vector2i Size, float width, float height)
        {
            float ndcX = (2.0f * mousePosition.X) / Size.X - 1.0f;
            float ndcY = 1.0f - (2.0f * mousePosition.Y) / Size.Y;
            ScreenPosition = new Vector2(ndcX, ndcY);

            WorldPosition = (
                cameraPosition.X + (ScreenPosition.X * width / 2.0f),
                cameraPosition.Y + (ScreenPosition.Y * height / 2.0f));
        }
    }
}
