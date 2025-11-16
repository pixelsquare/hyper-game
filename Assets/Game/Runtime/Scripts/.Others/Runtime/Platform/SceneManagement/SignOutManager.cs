using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Gifting;
using Kumu.Kulitan.Multiplayer;
using Kumu.Kulitan.UI;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.Common
{
    public class SignOutManager : SingletonMonoBehaviour<SignOutManager>
    {
        /// <summary>
        /// Logs the user out using the LogOut API.
        /// Sends the user back to sign up screen + resets settings.
        /// </summary>
        public void UbeSignOut()
        {
            SignOutAsync();
        }

        /// <summary>
        /// Sends the user to the sign up screen instantly without
        /// using the LogOut API. Resets settings.
        /// </summary>
        public void ReturnToSignInScreen()
        {
            Services.AuthService.LogOutUserAsync(new LogoutUserRequest());
            ResetAppSettings();
            LoadSignUpScreen();
        }

        public void LoadSignUpScreen()
        {
            ConnectionManager.Client.Disconnect();
            SceneManager.LoadScene(SceneNames.SIGNUP_SCENE);
        }

        public void ResetAppSettings()
        {
            BackendUtil.ClearAllTokens();
            UserInventoryData.Reset();
            if (VirtualGiftDatabase.Current)
            {
                VirtualGiftDatabase.Current.IsSynced = false;
            }
        }
        
        private async void SignOutAsync()
        {
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(true);
            var logOutResult = await Services.AuthService.LogOutUserAsync(new LogoutUserRequest());
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(false);

            if (logOutResult.HasError)
            {
                ShowErrorPopup(logOutResult.Error.Message);
                return;
            }

            ReturnToSignInScreen();
        }
        
        private void ShowErrorPopup(string message)
        {
            var errorPopup = PopupManager.Instance.OpenErrorPopup("ERROR", message, "OK");
            errorPopup.OnClosed += ReturnToSignInScreen;
        }

    }
}
