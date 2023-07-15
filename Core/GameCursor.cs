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
    }
}
