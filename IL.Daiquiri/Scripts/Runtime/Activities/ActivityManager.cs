using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IL.Daiquiri.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace IL.Daiquiri.Activities
{
    public sealed class ActivityManager : IModule
    {
        private readonly ActivityFactory _activityFactory;

        private Stack<Transition> _previousTransitions;

        [UsedImplicitly]
        public ActivityManager(ActivityFactory activityFactory)
        {
            _activityFactory = activityFactory;
        }

        public async UniTask ExecuteAsync(Transition transition, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentTransition = transition;

            while (true)
            {
                if (currentTransition.TransitionMode == TransitionMode.Activity)
                {
                    var activity = _activityFactory.Create(currentTransition.ActivityType);

                    activity.Initialize(currentTransition.Payload);

                    var nextTransition = await ExecuteAsync(activity, cancellationToken);

                    activity.Deinitialize();

                    if (nextTransition.TransitionMode == TransitionMode.Activity)
                    {
                        _previousTransitions.Push(currentTransition);
                    }

                    currentTransition = nextTransition;
                }
                else if (currentTransition.TransitionMode == TransitionMode.Back)
                {
                    if (!_previousTransitions.TryPop(out currentTransition))
                    {
                        break;
                    }
                }
                else if (currentTransition.TransitionMode == TransitionMode.Exit)
                {
                    break;
                }
            }
        }

        private static async UniTask<Transition> ExecuteAsync(IActivity activity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var transition = await activity.ExecuteAsync(cancellationToken);

                return transition;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);

                var transition = Transition.GoToBack();

                return transition;
            }
        }

        // TODO: Вынести в конфиг
        int IModule.Priority => 10;

        void IModule.Initialize()
        {
            _previousTransitions = new Stack<Transition>(16);
        }

        void IModule.Deinitialize()
        {
            _previousTransitions.Clear();
            _previousTransitions = null;
        }
    }
}
