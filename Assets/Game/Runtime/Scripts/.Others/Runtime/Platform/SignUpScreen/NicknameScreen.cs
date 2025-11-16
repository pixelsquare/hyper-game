using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Tracking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(UserProfileErrorHandler))]
    public class NicknameScreen : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputFieldNickname;
        [SerializeField] private Button btnSubmit;
        [SerializeField] private TextMeshProUGUI txtError;
        [SerializeField] private GameObject[] errorIndicators;

        [SerializeField] private UnityEvent onProfileSentSuccess;

        private UserProfileErrorHandler errorHandler;
        
        private IInputFormatValidator<UserNickNameFormatDetails> UserNickNameValidator =>
            Validators.UserNickNameValidator;
        private UserProfileLocalDataManager UserProfileLocalDataManager => UserProfileLocalDataManager.Instance;
        private LoadingScreenManager LoadingScreenManager => LoadingScreenManager.Instance;

        private async void SendUserProfileAsync(string name, string nickname, int ageRange, int gender)
        {
            HideError();
            LoadingScreenManager.ShowHideLoadingOverlay(true);
            var request = new CreateUserProfileRequest
            {
                username = name,
                nickname = nickname,
                ageRange = ageRange,
                gender = gender
            };
            var serviceResult = await Services.UserProfileService.CreateUserProfileAsync(request);

            if (serviceResult.HasError)
            {
                LoadingScreenManager.ShowHideLoadingOverlay(false);
                errorHandler.InvokeError(serviceResult.Error);
                return;
            }

            var userProfileResult = await Services.UserProfileService.GetUserProfileAsync(new GetUserProfileRequest());

            LoadingScreenManager.ShowHideLoadingOverlay(false);

            if (userProfileResult.HasError)
            {
                ShowError(BackendUtil.GetDisplayableErrorMessage(serviceResult.Error));
                return;
            }
            LoadingScreenManager.ShowHideLoadingOverlay(false);
            var userProfile = userProfileResult.Result.UserProfile;
            UserProfileLocalDataManager.UpdateLocalUserProfile(userProfile);

            onProfileSentSuccess?.Invoke();
        }

        private void ShowError(string error)
        {
            txtError.text = error;
            txtError.gameObject.SetActive(true);
        }

        private void HideError()
        {
            txtError.gameObject.SetActive(false);
        }
        
        private void UpdateErrorIndicators(UserNickNameFormatDetails details)
        {
            errorIndicators[0].SetActive(details.IsWithinCharLimits);
            errorIndicators[1].SetActive(details.HasNoSpecialChars);
            errorIndicators[2].SetActive(details.HasNoTrailingOrLeadingWhitespaces);
        }

        private void OnUserProfileSubmitted()
        {
            var username = UsernameScreen.username;
            var nickname = inputFieldNickname.text;
            var age = UserProfileAgeScreen.userProfileAge;
            var gender = UserProfileGenderScreen.userProfileGender;

            SendUserProfileAsync(username, nickname, age, gender);
        }

        private void OnNicknameUpdated(string value)
        {
            if (!UserNickNameValidator.IsValid(value, out var details))
            {
                btnSubmit.interactable = false;
                UpdateErrorIndicators(details);
                return;
            }
            UpdateErrorIndicators(details);
            btnSubmit.interactable = true;
        }

        private void Awake()
        {
            btnSubmit.interactable = false;
            errorHandler = GetComponent<UserProfileErrorHandler>();
        }

        private void OnEnable()
        {
            btnSubmit.onClick.AddListener(OnUserProfileSubmitted);
            inputFieldNickname.onValueChanged.AddListener(OnNicknameUpdated);
            inputFieldNickname.text = "";
            GlobalNotifier.Instance.Trigger(new UserJourneyEvent(UserJourneyEvent.Checkpoint.Nickname));
        }

        private void OnDisable()
        {
            btnSubmit.onClick.RemoveListener(OnUserProfileSubmitted);
            inputFieldNickname.onValueChanged.RemoveListener(OnNicknameUpdated);
        }
    }
}
