#if IL_DAIQUIRI_SPARSEINJECT_SUPPORT
using System.Linq;
using SparseInject;

namespace IL.Daiquiri.SparseInject
{
    public static class ScopeResolverExtensions
    {
        public static void InitializeModules(this IScopeResolver scopeResolver)
        {
            var modules = scopeResolver
                .Resolve<IModule[]>()
                .OrderBy(static module => module.Priority);

            foreach (var module in modules)
            {
                module.Initialize();
            }
        }

        public static void DeinitializeModules(this IScopeResolver scopeResolver)
        {
            var modules = scopeResolver
                .Resolve<IModule[]>()
                .OrderByDescending(static module => module.Priority);

            foreach (var module in modules)
            {
                module.Deinitialize();
            }
        }
    }
}
#endif
