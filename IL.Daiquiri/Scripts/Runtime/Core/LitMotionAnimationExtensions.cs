using Cysharp.Threading.Tasks;
using LitMotion.Animation;

namespace IL.Daiquiri.Core
{
    public static class LitMotionAnimationExtensions
    {
        public static void PlayOnce(this LitMotionAnimation animation)
        {
            PlayOnceAsync(animation).Forget();
        }

        private static async UniTaskVoid PlayOnceAsync(LitMotionAnimation animation)
        {
            animation.Play();

            await UniTask.WaitWhile(animation, static animation => animation.IsPlaying, cancellationToken: animation.destroyCancellationToken);

            animation.Stop();
        }
    }
}
