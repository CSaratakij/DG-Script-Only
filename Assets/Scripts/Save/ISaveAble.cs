namespace DG
{
    public interface ISaveAble
    {
        string Key { get; }
        uint ID { get; }

        void Save();
        void Load();
    }
}
