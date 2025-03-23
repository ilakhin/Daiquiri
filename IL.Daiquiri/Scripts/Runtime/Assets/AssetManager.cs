using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using R3;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace IL.Daiquiri.Assets
{
    public sealed class AssetManager
    {
        [UsedImplicitly]
        public AssetManager()
        {
        }

        public async UniTask<TObject> LoadAssetAsync<TObject>(string assetKey, LifetimeToken lifetimeToken, CancellationToken cancellationToken)
            where TObject : Object
        {
            var handle = Addressables.LoadAssetAsync<TObject>(assetKey);
            var asset = await handle.ToUniTask(cancellationToken: cancellationToken, cancelImmediately: true, autoReleaseWhenCanceled: true);
            var disposable = Disposable.Create(handle, Addressables.Release);

            lifetimeToken.Register(disposable);

            return asset;
        }
    }
}
