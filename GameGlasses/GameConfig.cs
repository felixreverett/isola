namespace FeloxGame.GameClasses
{
    public class GameConfig
    {
        public bool AllowSaving { get; set; }

        public GameConfig(bool allowSaving)
        {
            AllowSaving = allowSaving;
        }
    }
}
