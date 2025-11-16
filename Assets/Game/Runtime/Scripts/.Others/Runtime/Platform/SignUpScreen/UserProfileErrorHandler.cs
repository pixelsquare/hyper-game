using System;
using System.Collections;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Common
{
    public class UserProfileErrorHandler : MonoBehaviour
    {
        [SerializeField] private int errorPopupTimer = 3;
        [SerializeField] private UnityEvent onResetUserProfileSetting;
        [SerializeField] private UnityEvent onResetToSignUpScreen;

        private IPopup errorPopup;
        
        public void InvokeError(ServiceError error)
        {
            switch (error.Code)
            {
                case ServiceErrorCodes.INVALID_DATA:
                    StartCoroutine(ShowTimedErrorPopup(error.Message, () => onResetUserProfileSetting?.Invoke()));
                    break;
                case ServiceErrorCodes.PROFILE_ALREADY_EXISTS:
                    StartCoroutine(ShowTimedErrorPopup(error.Message, () => onResetToSignUpScreen?.Invoke()));
                    break;
                default:
                    StartCoroutine(ShowTimedErrorPopup(BackendUtil.GetDisplayableErrorMessage(error), null));
                    break;
            }
        }

        private void ShowErrorPopup(string message, Action callback = null)
        {
            errorPopup = PopupManager.Instance.OpenErrorPopup("Error", message, null);
            if (callback == null)
            {
                return;
            }
            errorPopup.OnClosed += callback;
        }

        private IEnumerator ShowTimedErrorPopup(string message, Action callback)
        {
            ShowErrorPopup(message, callback);
            yield return new WaitForSeconds(errorPopupTimer);
            errorPopup.Close();
        }
    }
}
