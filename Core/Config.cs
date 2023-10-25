namespace FeloxGame.Core
{
    public class Config
    {
        public bool AllowSaving { get; set; }

        public Config(bool allowSaving)
        {
            AllowSaving = allowSaving;
        }
    }
}
