using System;
using UnityEngine;

namespace IL.Daiquiri
{
    public static class LifetimeTokenUtility
    {
        private static LifetimeToken? _terminatedToken;

        public static LifetimeToken TerminatedToken => _terminatedToken ??= CreateTerminatedToken();

        public static event Action<Exception> UnobservedExceptionHandler;

        private static LifetimeToken CreateTerminatedToken()
        {
            var source = new LifetimeTokenSource();

            source.Terminate();

            return source.Token;
        }

        internal static void PublishUnobservedException(Exception exception)
        {
            if (UnobservedExceptionHandler == null)
            {
                Debug.LogException(exception);
            }
            else
            {
                UnobservedExceptionHandler.Invoke(exception);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _terminatedToken = null;

            UnobservedExceptionHandler = null;
        }
    }
}
