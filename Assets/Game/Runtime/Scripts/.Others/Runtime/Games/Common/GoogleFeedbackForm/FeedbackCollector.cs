using System.Collections;
using Kumu.Extensions;
using Kumu.Kulitan.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Common
{
    public class FeedbackCollector : MonoBehaviour
    {
        [SerializeField] private FeedbackEntryReferencesScriptableObject entryData;
        [SerializeField] private FeedbackInputData inputData;
        [SerializeField] private SubmitButtonHandler buttonReference;

        [Header("Popup Settings")]
        [SerializeField] private UnityEvent onSubmitted;
        [SerializeField] private string successHeader = "SUCCESS";
        [SerializeField] private string successMsg = "Feedback submitted. Thank you!";
        [SerializeField] private string failedHeader = "ERROR";
        [SerializeField] private string failedMsg = "Form submission failed. Please try again.";
        [SerializeField] private string confirmationHeader = "FORM SUBMISSION";
        [SerializeField] private string confirmationMsg = "Are you sure you want to submit the form?";

        private void GetInputData()
        {
            inputData = gameObject.AddComponent<FeedbackInputData>();

            inputData.Email = buttonReference.emailInputField.text;
            inputData.WirelessHeadphones = buttonReference.wirelessHeadPhoneInputField.text;
            inputData.SimilarApp = buttonReference.similarAppInputField.text;
            inputData.Crashes = buttonReference.crashesInputField.text;
            inputData.Comments = buttonReference.commentsInputField.text;
            inputData.EnjoyUbeRating = buttonReference.enjoyUbeToggleInput.toggleValue;
            inputData.GraphicsRating = buttonReference.graphicsToggleInput.toggleValue;
            inputData.IntuitiveRating = buttonReference.intuitiveToggleInput.toggleValue;
            inputData.AvatarDesignRating = buttonReference.avatarDesignToggleInput.toggleValue;
            inputData.JoinRoomsRating = buttonReference.joinRoomsToggleInput.toggleValue;
            inputData.MoveAvatarRating = buttonReference.moveAvatarToggleInput.toggleValue;
            inputData.EmotesRating = buttonReference.emotesToggleInput.toggleValue;
            inputData.VoiceChatRating = buttonReference.voiceChatToggleInput.toggleValue;
            inputData.RoomDesignRating = buttonReference.roomDesignToggleInput.toggleValue;
            inputData.PhonePerformanceRating = buttonReference.phonePerformanceToggleInput.toggleValue;
            inputData.UbeSatisfactionRating = buttonReference.ubeSatifactionToggleInput.toggleValue;
            inputData.Nda = buttonReference.ndaToggleInput.toggleValue;
        }

        private IEnumerator SendGFormData()
        {
            WWWForm form = new WWWForm();

            // Input fields 
            form.AddField(entryData.KumuEmailEntryId, inputData.Email);
            form.AddField(entryData.WirelessHeadphonesEntryId, inputData.WirelessHeadphones);
            form.AddField(entryData.SimilarAppEntryId, inputData.SimilarApp);
            form.AddField(entryData.CrashesEntryId, inputData.Crashes);
            form.AddField(entryData.CommentsEntryId, inputData.Comments);
            // Device info fields
            form.AddField(entryData.DeviceModelEntryId, SystemInfo.deviceModel);
            form.AddField(entryData.OperatingSystemEntryId, SystemInfo.operatingSystem);
            form.AddField(entryData.SystemMemorySizeEntryId, SystemInfo.systemMemorySize);
            form.AddField(entryData.GraphicsDeviceNameEntryId, SystemInfo.graphicsDeviceName);
            form.AddField(entryData.GraphicsMemorySizeEntryId, SystemInfo.graphicsMemorySize);
            form.AddField(entryData.GraphicsShaderLevelEntryId, SystemInfo.graphicsShaderLevel);
            // Ratings 
            form.AddField(entryData.EnjoyPlayingUbeEntryId, inputData.EnjoyUbeRating);
            form.AddField(entryData.LikeGraphicsUbeEntryId, inputData.GraphicsRating);
            form.AddField(entryData.IntuitiveEntryId, inputData.IntuitiveRating);
            form.AddField(entryData.AvatarDesignEntryId, inputData.AvatarDesignRating);
            form.AddField(entryData.JoinRoomsEntryId, inputData.JoinRoomsRating);
            form.AddField(entryData.MoveAvatarEntryId, inputData.MoveAvatarRating);
            form.AddField(entryData.EmotesEntryId, inputData.EmotesRating);
            form.AddField(entryData.VoiceChatEntryId, inputData.VoiceChatRating);
            form.AddField(entryData.RoomDesignEntryId, inputData.RoomDesignRating);
            form.AddField(entryData.PhonePerformanceEntryId, inputData.PhonePerformanceRating);
            form.AddField(entryData.UbeSatisfactionEntryId, inputData.UbeSatisfactionRating);
            //nda
            form.AddField(entryData.Nda, inputData.Nda);

            using UnityWebRequest www = UnityWebRequest.Post(entryData.FormBaseURL, form );
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(true);
            yield return www.SendWebRequest();
            PlayerPrefs.SetInt("Feedback Form", 1);
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(false);

            if (www.result == UnityWebRequest.Result.Success)
            {
                ShowSubmitResultPopup(true);
                $"<color=#FFC0CB>{UnityWebRequest.Result.Success}</color>".Log();
                yield break;
            }
            ShowSubmitResultPopup(false);
            $"<color=#FFC0CB>{www.error}</color>".Log();
        }

        public void OnClickSubmitFormData()
        {
            AttemptFormSubmission();
        }

        private void AttemptFormSubmission()
        {
            var popup = (ConfirmationPopup)PopupManager.Instance.OpenConfirmationPopup(confirmationHeader,
                confirmationMsg, "SUBMIT", "CANCEL");
            popup.OnConfirm += SubmitData;
        }

        private void SubmitData()
        {
            GetInputData();
            StartCoroutine(SendGFormData());
        }

        private void ShowSubmitResultPopup(bool isSuccessful)
        {
            var title = isSuccessful ? successHeader : failedHeader;
            var msg = isSuccessful ? successMsg : failedMsg;
            var popup = PopupManager.Instance.OpenTextPopup(title, msg, "OK");

            if (!isSuccessful)
            {
                return;
            }

            popup.OnClosed += () => onSubmitted.Invoke();
        }
    }
}
