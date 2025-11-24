using System;
using Cysharp.Threading.Tasks;
using Santelmo.Rinsurv.Backend;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using SceneName = GameConstants.SceneNames;
    using AppStateEvent = GameEvents.AppState;

    public class LoginState : State
    {
        [Inject] private ISceneManager _sceneManager;
        [Inject] private IAuthService _authService;
        [Inject] private IUserAccountManager _userAccountManager;
        [Inject] private LazyInject<IHudManager> _hudManager;
        [Inject] private LazyInject<IPopupManager> _popupManager;
        [Inject] private LazyInject<IOverlayManager> _overlayManager;

        public override async void OnEnter()
        {
            Dispatcher.AddListener(AppStateEvent.OnGoogleLoginEvent, HandleGoogleSignInEvent);
            Dispatcher.AddListener(AppStateEvent.OnAppleLoginEvent, HandleAppleSignInEvent);
            Dispatcher.AddListener(AppStateEvent.OnFacebookLoginEvent, HandleFacebookSignInEvent);
            Dispatcher.AddListener(AppStateEvent.OnGuestLoginEvent, HandleGuestSignInEvent);

            if (!_sceneManager.IsSceneLoaded(SceneName.UiScene) 
             && !_sceneManager.IsSceneLoaded(SceneName.ManagerScene))
            {
                await _sceneManager.LoadSceneAdditiveAsync(
                    SceneName.UiScene,
                    SceneName.ManagerScene).Task;
            }

            await UniTask.WaitUntil(() => _hudManager.Value.IsInitialized);

            _hudManager.Value.ShowHud(HudType.Login);
            _sceneManager.SetSceneActive(SceneName.UiScene);
            _sceneManager.UnloadSceneAsync(SceneName.AssetLoaderScene);

            if (_authService.IsAlreadySigned)
            {
                await InitializeUserAccountAsync();
                EndState();
            }
        }

        public override void OnExit()
        {
            Dispatcher.RemoveListener(AppStateEvent.OnGoogleLoginEvent, HandleGoogleSignInEvent, true);
            Dispatcher.RemoveListener(AppStateEvent.OnAppleLoginEvent, HandleAppleSignInEvent, true);
            Dispatcher.RemoveListener(AppStateEvent.OnFacebookLoginEvent, HandleFacebookSignInEvent, true);
            Dispatcher.RemoveListener(AppStateEvent.OnGuestLoginEvent, HandleGuestSignInEvent, true);

            _hudManager.Value.HideHud(HudType.Login);
        }

        private async void HandleGoogleSignInEvent(IMessage message)
        {
            var loginTask = LoginUserGoogleAsync();

            if (!await TryUserSignInAsync(loginTask))
            {
                ShowLoginFailedPopup();
                return;
            }

            EndState();
        }

        private void HandleAppleSignInEvent(IMessage message)
        {
        }

        private async void HandleFacebookSignInEvent(IMessage message)
        {
            if (!await TryLoginUserFacebook())
            {
                ShowLoginFailedPopup();
                return;
            }

            EndState();
        }

        private async void HandleGuestSignInEvent(IMessage message)
        {
            var loginTask = LoginUserAnonymousAsync();

            if (!await TryUserSignInAsync(loginTask))
            {
                ShowLoginFailedPopup();
                return;
            }

            EndState();
        }

        private async void ShowLoginFailedPopup()
        {
            var error = Application.internetReachability == NetworkReachability.NotReachable
                ? "\nUnable to connect to the internet"
                : "";
            var prompt = await _popupManager.Value.ShowGenericPopupAsync(GenericPopup.ModalType.Default,
                $"Failed to log in. {error}\nPlease try again.",
                GenericPopup.ConfirmationOption.Close);
            
            Dispatcher.SendMessage(AppStateEvent.OnSignInErrorEvent);
        }

        private async UniTask<bool> TryUserSignInAsync(UniTask<bool> loginTask)
        {
            try
            {
                _overlayManager.Value.ShowOverlay("Logging in...");
                var result = await loginTask;
                await InitializeUserAccountAsync();
                _overlayManager.Value.HideOverlay();
                return result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async UniTask<bool> LoginUserAnonymousAsync()
        {
            try
            {
                var result = await _authService.SignInAnonymouslyAsync(new SignInAnonymouslyRequest());
                return !result.HasError;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async UniTask<bool> LoginUserGoogleAsync()
        {
            try
            {
                var googleSignInResult = await _authService.SignInGoogleAsync(new SignInGoogleRequest());
                return !googleSignInResult.HasError;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async UniTask<bool> TryLoginUserFacebook()
        {
            try
            {
                var facebookSignInResult = await _authService.SignInFacebookAsync(new SignInFacebookRequest());
                return !facebookSignInResult.HasError;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async UniTask<bool> TryLoginUserEmail(string email, string password)
        {
            try
            {
                _overlayManager.Value.ShowOverlay("Logging in...");
                var emailSignInResult = await _authService.SignInEmailAsync(new SignInEmailRequest(email, password));
                _overlayManager.Value.HideOverlay();
                return !emailSignInResult.HasError;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async UniTask<bool> TryCreateUserEmail(string email, string password)
        {
            try
            {
                _overlayManager.Value.ShowOverlay("Creating user...");
                var emailSignInResult = await _authService.CreateUserWithEmailAsync(new CreateUserWithEmailRequest(email, password));
                _overlayManager.Value.HideOverlay();
                return !emailSignInResult.HasError;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async UniTask<bool> TrySendPasswordReset(string email)
        {
            try
            {
                _overlayManager.Value.ShowOverlay("Sending password reset...");
                var emailSignInResult = await _authService.SendPasswordResetAsync(new SendPasswordResetRequest(email));
                _overlayManager.Value.HideOverlay();
                return !emailSignInResult.HasError;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async UniTask InitializeUserAccountAsync()
        {
            try
            {
                _overlayManager.Value.ShowOverlay("Initializing Account ...");
                await _userAccountManager.InitializeAsync();
                _overlayManager.Value.HideOverlay();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}
