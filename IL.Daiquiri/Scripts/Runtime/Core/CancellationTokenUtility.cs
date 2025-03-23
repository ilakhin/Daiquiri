using System.Threading;
using UnityEngine;

namespace IL.Daiquiri.Core
{
    public static class CancellationTokenUtility
    {
        private static CancellationToken? _cancelledToken;

        public static CancellationToken CancelledToken => _cancelledToken ??= CreateCancelledToken();

        private static CancellationToken CreateCancelledToken()
        {
            var source = new CancellationTokenSource();

            source.Cancel();

            return source.Token;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _cancelledToken = null;
        }
    }
}
