namespace FeloxGame.GameClasses
{
    public class GameConfig
    {
        public bool AllowSaving { get; set; }
        public int RenderDistance { get; set; }

        public GameConfig(bool allowSaving, int renderDistance)
        {
            AllowSaving = allowSaving;
            RenderDistance = renderDistance;
        }
    }
}
