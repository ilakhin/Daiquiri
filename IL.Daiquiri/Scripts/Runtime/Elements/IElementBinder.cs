namespace IL.Daiquiri.Elements
{
    public interface IElementBinder
    {
        void Initialize(IElementViewModel viewModel);

        void Deinitialize();
    }
}
