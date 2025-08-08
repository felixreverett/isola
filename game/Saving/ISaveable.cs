namespace Isola.Saving
{
    public interface ISaveable<T>
    {
        T GetSaveData();
    }
}
