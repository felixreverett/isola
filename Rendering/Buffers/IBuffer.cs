namespace FeloxGame.Rendering
{
    public interface IBuffer
    {
        int BufferId { get; }
        void Bind();
        void Unbind();
    }
}
