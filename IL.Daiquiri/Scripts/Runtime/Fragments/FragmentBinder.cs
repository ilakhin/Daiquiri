using System.Threading;
using Cysharp.Threading.Tasks;
using IL.Daiquiri.Elements;

namespace IL.Daiquiri.Fragments
{
    public class FragmentBinder : FragmentBinder<FragmentViewModel, FragmentPayload>
    {
    }

    public class FragmentBinder<TViewModel> : FragmentBinder<TViewModel, FragmentPayload>
        where TViewModel : FragmentViewModel<FragmentPayload>
    {
    }

    public class FragmentBinder<TViewModel, TPayload> : ElementBinder<TViewModel, TPayload>
        where TViewModel : FragmentViewModel<TPayload>
        where TPayload : FragmentPayload
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();

            OnShow();
        }

        protected override void OnDeinitialize()
        {
            if (this == null)
            {
                return;
            }

            var hideDuration = OnHide();

            if (hideDuration > 0f)
            {
                OnDeinitializeAsync(hideDuration, CancellationToken).Forget();

                return;
            }

            base.OnDeinitialize();
        }

        private async UniTaskVoid OnDeinitializeAsync(float delay, CancellationToken cancellationToken)
        {
            await UniTask.WaitForSeconds(delay, cancellationToken: cancellationToken);

            if (this == null)
            {
                return;
            }

            base.OnDeinitialize();
        }

        protected virtual float OnShow()
        {
            return 0f;
        }

        protected virtual float OnHide()
        {
            return 0f;
        }
    }
}
