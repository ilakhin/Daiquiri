namespace IL.Daiquiri
{
    public interface IModule
    {
        int Priority
        {
            get;
        }

        void Initialize();

        void Deinitialize();
    }
}
