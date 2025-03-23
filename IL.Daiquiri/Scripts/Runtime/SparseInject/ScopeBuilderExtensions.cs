#if IL_DAIQUIRI_SPARSEINJECT_SUPPORT
using System;
using IL.Daiquiri.Activities;
using IL.Daiquiri.Assets;
using IL.Daiquiri.Blackout;
using IL.Daiquiri.Commands;
using IL.Daiquiri.Elements;
using IL.Daiquiri.Fragments;
using IL.Daiquiri.Operations;
using IL.Daiquiri.Popups;
using IL.Daiquiri.Screens;
using IL.Daiquiri.Storage;
using IL.Daiquiri.Threads;
using SparseInject;
using UnityEngine;

namespace IL.Daiquiri.SparseInject
{
    public static class ScopeBuilderExtensions
    {
        public static void InstallActivities(this IScopeBuilder scopeBuilder)
        {
            scopeBuilder.Register<ActivityManager, IModule, ActivityManager>(Lifetime.Singleton);
            scopeBuilder.Register<ActivityFactory>(Lifetime.Singleton);
            scopeBuilder.RegisterFactory<Type, IActivity>(scope =>
            {
                return type =>
                {
                    var activity = (IActivity)scope.Resolve(type);

                    return activity;
                };
            });
        }

        public static void InstallAssets(this IScopeBuilder scopeBuilder)
        {
            scopeBuilder.Register<AssetManager>(Lifetime.Singleton);
        }

        public static void InstallBlackout(this IScopeBuilder scopeBuilder)
        {
            scopeBuilder.Register<BlackoutManager, IModule, BlackoutManager>(Lifetime.Singleton);
            scopeBuilder.Register<BlackoutViewModel>();
        }

        public static void InstallCommands(this IScopeBuilder scopeBuilder)
        {
            scopeBuilder.Register<CommandManager, IModule, CommandManager>(Lifetime.Singleton);
            scopeBuilder.Register<IModule, CommandHandlerRegistrar>(Lifetime.Singleton);
            scopeBuilder.Register<ICommandHandler, DummyCommandHandler>();
        }

        public static void InstallElements(this IScopeBuilder scopeBuilder)
        {
            scopeBuilder.Register<ElementManager, IModule, ElementManager>(Lifetime.Singleton);
            scopeBuilder.Register<ElementViewModel>();
            scopeBuilder.Register<ElementViewModelFactory>(Lifetime.Singleton);
            scopeBuilder.RegisterFactory<Type, IElementViewModel>(scope =>
            {
                return type =>
                {
                    var viewModel = (IElementViewModel)scope.Resolve(type);

                    return viewModel;
                };
            });
        }

        public static void InstallFragments(this IScopeBuilder scopeBuilder, Transform parentTransform)
        {
            scopeBuilder.Register<FragmentManager, IModule, FragmentManager>(Lifetime.Singleton);
            scopeBuilder.Register<FragmentViewModel>();
            scopeBuilder.Register<FragmentProjector, IModule, FragmentProjector>(Lifetime.Singleton);
            scopeBuilder.RegisterValue(new ParentTransformProvider(parentTransform));
        }

        public static void InstallOperations(this IScopeBuilder scopeBuilder)
        {
            scopeBuilder.Register<OperationManager, IModule, OperationManager>(Lifetime.Singleton);
            scopeBuilder.Register<IModule, OperationHandlerRegistrar>(Lifetime.Singleton);
            scopeBuilder.Register<IOperationHandler, DummyOperationHandler>();
        }

        public static void InstallPopups(this IScopeBuilder scopeBuilder)
        {
            scopeBuilder.Register<PopupManager, IModule, PopupManager>(Lifetime.Singleton);
            scopeBuilder.Register<PopupViewModel>();
        }

        public static void InstallScreens(this IScopeBuilder scopeBuilder)
        {
            scopeBuilder.Register<ScreenManager, IModule, ScreenManager>(Lifetime.Singleton);
            scopeBuilder.Register<ScreenViewModel>();
        }

        public static void InstallStorage(this IScopeBuilder scopeBuilder)
        {
            scopeBuilder.Register<StorageManager>(Lifetime.Singleton);
        }

        public static void InstallThreads(this IScopeBuilder scopeBuilder)
        {
            scopeBuilder.Register<ThreadManager, IModule, ThreadManager>(Lifetime.Singleton);
        }
    }
}
#endif
