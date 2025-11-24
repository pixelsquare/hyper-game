using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Santelmo.Rinsurv
{
    public class ReactiveTextureProperty : ReactiveProperty<string>
    {
        private Action OnStart;
        private Action OnEnd;

        private Sprite _value;
        private AsyncOperationHandle<Sprite> _lastAvatarSpriteHandle;

        private IDisposable _spriteDisposable;
        private IDisposable _spriteNameDisposable;

        private readonly IAssetLoaderManager _assetLoaderManager;
        private readonly ReactiveProperty<Sprite> _spriteProp = new(null);

        public ReactiveTextureProperty(IAssetLoaderManager assetLoaderManager)
        {
            _assetLoaderManager = assetLoaderManager;
        }

        public ReactiveTextureProperty Subscribe(Action<Sprite> onNext, Action onStart = null, Action onEnd = null)
        {
            OnStart = onStart;
            OnEnd = onEnd;
            _spriteDisposable?.Dispose();
            _spriteDisposable = _spriteProp.Subscribe(onNext);
            return this;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _spriteProp.Dispose();
            _spriteDisposable.Dispose();
            _spriteNameDisposable.Dispose();
            ReleaseAvatarSprite();
        }

        protected override void SetValue(string value)
        {
            base.SetValue(value);
            _spriteNameDisposable?.Dispose();
            _spriteNameDisposable = this.Subscribe(HandleAvatarProperty);
        }

        private async void HandleAvatarProperty(string spriteName)
        {
            _spriteProp.Value = null;

            if (string.IsNullOrEmpty(spriteName))
            {
                return;
            }

            try
            {
                ReleaseAvatarSprite();

                OnStart?.Invoke();

                spriteName = _assetLoaderManager.ResolveAssetName(spriteName);
                _lastAvatarSpriteHandle = Addressables.LoadAssetAsync<Sprite>(spriteName);
                var spriteLoadTask = _lastAvatarSpriteHandle.Task.AsUniTask();
                var timeoutTask = UniTask.WaitForSeconds(1.0f);
                var avatarSprite = await UniTask.WhenAny(spriteLoadTask, timeoutTask);

                _spriteProp.Value = avatarSprite.result;
                OnEnd?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private void ReleaseAvatarSprite()
        {
            if (_lastAvatarSpriteHandle.IsValid())
            {
                Addressables.Release(_lastAvatarSpriteHandle);
            }
        }
    }
}
