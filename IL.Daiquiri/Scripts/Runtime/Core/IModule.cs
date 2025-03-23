namespace IL.Daiquiri.Core
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
