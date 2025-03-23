using System.Threading;
using Cysharp.Threading.Tasks;

namespace IL.Daiquiri.Activities
{
    public interface IActivity
    {
        void Initialize(Payload payload);

        void Deinitialize();

        UniTask<Transition> ExecuteAsync(CancellationToken cancellationToken);
    }
}
