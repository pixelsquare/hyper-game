using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public static class PopupExtensions
    {
        public static async UniTask<GenericPopup.ConfirmationOption> ShowGenericPopupAsync(this IPopupManager popupManager,
                                                                                           GenericPopup.ModalType modalType,
                                                                                           string message,
                                                                                           GenericPopup.ConfirmationOption buttons)
        {
            return await popupManager.ShowPopupAsync<GenericPopup>(PopupType.Generic)
                                     .Setup(modalType, message, buttons)
                                     .Task;
        }
    }
}
