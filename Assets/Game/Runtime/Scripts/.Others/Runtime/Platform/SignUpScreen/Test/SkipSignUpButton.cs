using System;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(Button))]
    public class SkipSignUpButton : MonoBehaviour
    {
        private Button button;
        private UserProfileLocalDataManager UserProfileLocalDataManager => UserProfileLocalDataManager.Instance;
        
        private void OnSkipClicked()
        {
            InitializeDefaultUserProfile();
            GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("OnSignUpSkipped"));
        }

        private void InitializeDefaultUserProfile()
        {
            var userProfile = new UserProfile
            {
                accountId = Guid.NewGuid().ToString(),
                nickName = "I skipped Sign Up",
#if  USES_MOCKS
                hasLinkedKumuAccount = true,          
#else
                hasLinkedKumuAccount = false,
#endif
            };
            
            UserProfileLocalDataManager.UpdateLocalUserProfile(userProfile);
        }

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void Start()
        {
            #if UNITY_EDITOR
            gameObject.SetActive(true);
            #else
            gameObject.SetActive(false);
            #endif
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnSkipClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnSkipClicked);
        }
    }
}
