using System;
using JetBrains.Annotations;

namespace IL.Daiquiri.Elements
{
    public sealed class ElementViewModelFactory
    {
        private readonly Func<Type, IElementViewModel> _func;

        [UsedImplicitly]
        public ElementViewModelFactory(Func<Type, IElementViewModel> func)
        {
            _func = func;
        }

        public IElementViewModel Create(Type viewModelType)
        {
            var viewModel = _func(viewModelType);

            return viewModel;
        }
    }
}
