using OpenTK.Mathematics;

namespace FeloxGame.Core
{
    public class GameCursor
    {
        public Vector2 Position { get; set; }

        public int Rounded(float f)
        {
            return (int)Math.Floor(f);
        }

        public void UpdatePosition(Vector2 mousePosition, Vector3 cameraPosition, Vector2i Size, float width, float height)
        {
            float ndcX = (2.0f * mousePosition.X) / Size.X - 1.0f;
            float ndcY = 1.0f - (2.0f * mousePosition.Y) / Size.Y;
            Vector2 ndcCursorPos = new Vector2(ndcX, ndcY);

            Position = (
                cameraPosition.X + (ndcCursorPos.X * width / 2.0f),
                cameraPosition.Y + (ndcCursorPos.Y * height / 2.0f));
        }
    }
}
