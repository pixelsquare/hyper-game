using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Santelmo.Rinsurv.Backend;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using SceneName = GameConstants.SceneNames;
    using AppStateEvent = GameEvents.AppState;

    public class MainMenuState : State
    {
        [Inject] private IHudManager _hudManager;
        [Inject] private ISceneManager _sceneManager;
        [Inject] private IPopupManager _popupManager;
        [Inject] private IAudioManager _audioManager;
        [Inject] private IAuthService _authService;

        private bool _didShowAppExitPrompt;

        public override async void OnEnter()
        {
            Dispatcher.AddListener(AppStateEvent.ToMissionSelectScreenEvent, HandleMissionSelectScreenEvent);
            Dispatcher.AddListener(AppStateEvent.ToHeroesScreenEvent, HandleHeroesScreenEvent);
            Dispatcher.AddListener(AppStateEvent.ToGearScreenEvent, HandleGearScreenEvent);
            Dispatcher.AddListener(AppStateEvent.ToInventoryScreenEvent, HandleInventoryScreenEvent);
            Dispatcher.AddListener(AppStateEvent.ToShopScreenEvent, HandleShopScreenEvent);
            Dispatcher.AddListener(AppStateEvent.OnUserSignOutEvent, HandleUserSignOutEvent);

            if (!_sceneManager.IsSceneLoaded(SceneName.WorldScene))
            {
                await _sceneManager.LoadSceneAdditiveAsync(SceneName.WorldScene).Task;
            }

            _hudManager.ShowHud(HudType.MainMenu);
            _audioManager.PlaySound(Bgm.MainMenu);
        }

        public override void OnExit()
        {
            _hudManager.HideHud(HudType.MainMenu);
            _audioManager.StopSound(Bgm.MainMenu);
            Dispatcher.RemoveListener(AppStateEvent.ToMissionSelectScreenEvent, HandleMissionSelectScreenEvent, true);
            Dispatcher.RemoveListener(AppStateEvent.ToHeroesScreenEvent, HandleHeroesScreenEvent, true);
            Dispatcher.RemoveListener(AppStateEvent.ToGearScreenEvent, HandleGearScreenEvent, true);
            Dispatcher.RemoveListener(AppStateEvent.ToInventoryScreenEvent, HandleInventoryScreenEvent, true);
            Dispatcher.RemoveListener(AppStateEvent.ToShopScreenEvent, HandleShopScreenEvent, true);
            Dispatcher.RemoveListener(AppStateEvent.OnUserSignOutEvent, HandleUserSignOutEvent, true);
        }

        public override async UniTask<bool> OnRollbackAsync()
        {
            if (!CanShowAppExitPrompt())
            {
                return true;
            }

            if (_didShowAppExitPrompt)
            {
                return false;
            }

            try
            {
                _didShowAppExitPrompt = true;
                var prompt = await _popupManager.ShowGenericPopupAsync(GenericPopup.ModalType.Default,
                    "Do you want to exit the app?",
                    GenericPopup.ConfirmationOption.Cancel | GenericPopup.ConfirmationOption.Confirm);

                _didShowAppExitPrompt = false;
                if (prompt != GenericPopup.ConfirmationOption.Confirm)
                {
                    return false;
                }

                GameUtil.ExitApplication();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private void HandleMissionSelectScreenEvent(IMessage message)
        {
            EndState(AppStateEvent.ToMissionSelectScreenEvent, true);
        }

        private void HandleHeroesScreenEvent(IMessage message)
        {
            EndState(AppStateEvent.ToHeroesScreenEvent);
        }

        private void HandleGearScreenEvent(IMessage message)
        {
            EndState(AppStateEvent.ToGearScreenEvent);
        }

        private void HandleInventoryScreenEvent(IMessage message)
        {
            EndState(AppStateEvent.ToInventoryScreenEvent);
        }

        private void HandleShopScreenEvent(IMessage message)
        {
            EndState(AppStateEvent.ToShopScreenEvent);
        }

        private async void HandleUserSignOutEvent(IMessage message)
        {
            var prompt = await _popupManager.ShowGenericPopupAsync(GenericPopup.ModalType.Warning,
                "You are about to logout. Proceed?",
                GenericPopup.ConfirmationOption.Cancel | GenericPopup.ConfirmationOption.Confirm);

            if (prompt != GenericPopup.ConfirmationOption.Confirm)
            {
                return;
            }

            var task = await _authService.SignOutAsync(new SignOutRequest());

            EndState(AppStateEvent.ToLoginScreenEvent, true);
            _hudManager.HideHud(HudType.Settings);
        }

        private bool CanShowAppExitPrompt()
        {
            var androidProvider = (from provider in Fsm.ParentStateMachine.Providers
                                   where provider is AndroidOEMProvider
                                   select provider as AndroidOEMProvider).First();

            return androidProvider?.RollbackStackCount <= 0;
        }
    }
}
