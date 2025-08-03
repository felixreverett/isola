using OpenTK.Mathematics;

namespace FeloxGame.GameClasses
{
    public class GameCamera
    {
        
        // window dimensions
        private float left;
        private float right;
        private float top;
        private float bottom;
        public float Width { get; private set; }
        public float Height { get; private set; }

        public GameCamera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
            UpdateCameraDimensions();
        }

        /// Properties
        public Vector3 Position { get; set; }
        public float AspectRatio { get; set; }
        
        /// Methods
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.CreateTranslation(-Position);
        }

        public Matrix4 GetProjectionMatrix()
        {   
            return Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, -1.0f, 1.0f);
        }

        public void UpdateCameraDimensions()
        {
            Width = 40f;
            Height = Width / AspectRatio;
            left = -Width / 2.0f;
            right = Width / 2.0f;
            bottom = -Height / 2.0f;
            top = Height / 2.0f;
        }
    }
}
