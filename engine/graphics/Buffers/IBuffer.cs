namespace FeloxGame.Drawing
{
    public interface IBuffer
    {
        int BufferId { get; }
        void Bind();
        void Unbind();
    }
}
