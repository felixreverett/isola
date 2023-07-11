using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace FeloxGame
{
    /// <summary>
    /// All entities have a pos, size,
    /// </summary>
    public class Entity
    {
        public Vector2 Position { get; }
        public Vector2 Size { get; }

        public void Update(FrameEventArgs args) { }
        public void Draw(FrameEventArgs args) { }
    }
}
