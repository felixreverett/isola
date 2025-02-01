namespace FeloxGame.Saving
{
    public interface ISaveable<T>
    {
        T GetSaveData();
    }
}
