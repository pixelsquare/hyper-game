using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;

    public class LoginHud : BaseHud
    {
        [SerializeField] private RinawaButton _googleSignInButton;
        [SerializeField] private RinawaButton _appleSignInButton;
        [SerializeField] private RinawaButton _facebookSignInButton;
        [SerializeField] private RinawaButton _guestSignInButton;

        private CompositeDisposable _compositeDisposable = new();

        private void HandleGoogleSignInBtnClicked()
        {
            _googleSignInButton.interactable = false;
            Dispatcher.SendMessage(AppStateEvent.OnGoogleLoginEvent);
        }

        private void HandleAppleSignInBtnClicked()
        {
            _appleSignInButton.interactable = false;
            Dispatcher.SendMessage(AppStateEvent.OnAppleLoginEvent);
        }

        private void HandleFacebookSignInBtnClicked()
        {
            _facebookSignInButton.interactable = false;
            Dispatcher.SendMessage(AppStateEvent.OnFacebookLoginEvent);
        }

        private void HandleGuestSignInBtnClicked()
        {
            _guestSignInButton.interactable = false;
            Dispatcher.SendMessage(AppStateEvent.OnGuestLoginEvent);
        }

        private void HandleSignInErrorEvent(IMessage message)
        {
            _googleSignInButton.interactable = true;
            _appleSignInButton.interactable = true;
            _facebookSignInButton.interactable = true;
            _guestSignInButton.interactable = true;
        }

        private void Start()
        {
#if !UNITY_EDITOR
            _googleSignInButton.gameObject.SetActive(Application.platform == RuntimePlatform.Android);
            _appleSignInButton.gameObject.SetActive(Application.platform == RuntimePlatform.IPhonePlayer);
#endif
        }

        private void OnEnable()
        {
            _googleSignInButton.OnClickAsObservable()
                               .Subscribe(_ => { HandleGoogleSignInBtnClicked(); })
                               .AddTo(_compositeDisposable);

            _appleSignInButton.OnClickAsObservable()
                              .Subscribe(_ => { HandleAppleSignInBtnClicked(); })
                              .AddTo(_compositeDisposable);

            _facebookSignInButton.OnClickAsObservable()
                                 .Subscribe(_ => { HandleFacebookSignInBtnClicked(); })
                                 .AddTo(_compositeDisposable);

            _guestSignInButton.OnClickAsObservable()
                              .Subscribe(_ => { HandleGuestSignInBtnClicked(); })
                              .AddTo(_compositeDisposable);
            
            Dispatcher.AddListener(AppStateEvent.OnSignInErrorEvent, HandleSignInErrorEvent);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(AppStateEvent.OnGuestLoginEvent, HandleSignInErrorEvent, true);
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
