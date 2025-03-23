using System;

namespace IL.Daiquiri.Activities
{
    public sealed class Transition
    {
        public readonly TransitionMode TransitionMode;
        public readonly Type ActivityType;
        public readonly Payload Payload;

        private Transition(TransitionMode transitionMode, Type activityType, Payload payload)
        {
            TransitionMode = transitionMode;
            ActivityType = activityType;
            Payload = payload;
        }

        public static Transition GoToActivity<TActivity>()
            where TActivity : Activity<Payload>
        {
            var payload = new Payload();

            return new Transition(TransitionMode.Activity, typeof(TActivity), payload);
        }

        public static Transition GoToActivity<TActivity, TPayload>(TPayload payload)
            where TActivity : Activity<TPayload>
            where TPayload : Payload
        {
            return new Transition(TransitionMode.Activity, typeof(TActivity), payload);
        }

        public static Transition GoToExit()
        {
            return new Transition(TransitionMode.Exit, null, null);
        }

        public static Transition GoToBack()
        {
            return new Transition(TransitionMode.Back, null, null);
        }
    }
}
