using System;
using JetBrains.Annotations;

namespace IL.Daiquiri.Activities
{
    public sealed class ActivityFactory
    {
        private readonly Func<Type, IActivity> _func;

        [UsedImplicitly]
        public ActivityFactory(Func<Type, IActivity> func)
        {
            _func = func;
        }

        public IActivity Create(Type activityType)
        {
            var transition = _func(activityType);

            return transition;
        }
    }
}
