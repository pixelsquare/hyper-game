using System;
using System.Text;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class AccountBannedPopup : BasePopup
    {
        [SerializeField] private TMP_Text lblMessage;
        [SerializeField] private Toggle optoutToggle;

        private const string OptoutAgoraKey = "OptoutAgoraBanPopup";

        private bool OptoutAgoraBanPopup
        {
            get => Convert.ToBoolean(PlayerPrefs.GetInt(OptoutAgoraKey, 0));
            set => PlayerPrefs.SetInt(OptoutAgoraKey, Convert.ToInt32(value));
        }

        private bool isInitialized;
        private string messageFormat;
        private string banTypeString;
        private string causes;
        private string liftTime;
        private BanType banType;

        private string LiftTime
        {
            get
            {
                if (string.IsNullOrEmpty(liftTime))
                {
                    return "Indefinite";
                }

                var dt = DateTime.Parse(liftTime).Subtract(DateTime.Now);
                return dt.TotalSeconds > 0 
                        ? $"{dt.Days:00}d {dt.Hours:00}h {dt.Minutes:00}m {dt.Seconds:00}s" 
                        : "00d 00h 00m 00s";
            }
        }

        public void Initialize(BanType banType, string[] causes, string liftTime)
        {
            if (banType == BanType.Agora && OptoutAgoraBanPopup)
            {
                ClosePopup();
                return;
            }

            this.banType = banType;
            this.liftTime = liftTime;

            var sb = new StringBuilder();
            
            if (causes.Length > 0)
            {
                sb.Append("\n");
                sb.Append("● ");
                sb.AppendJoin("\n● ", causes);
                sb.Append("\n");
            }
            
            this.causes = sb.ToString();

            banTypeString = banType switch
            {
                BanType.Login => "log in",
                BanType.Agora => "use text or voice chat",
                _ => string.Empty
            };

            SetMessageDirty();
            SetOptoutToggleActive(banType == BanType.Agora);
            isInitialized = true;
        }

        public void SignOut()
        {
            var isSignUpScreenActive = SceneLoadingManager.Instance.IsSceneActive(SceneNames.SIGNUP_SCENE);

            if (banType != BanType.Login || isSignUpScreenActive)
            {
                return;
            }

            SignOutManager.Instance.ReturnToSignInScreen();
        }

        public void ContactCustomerSupport()
        {
            SendEmail("welp@enjoyube.com", "I am banned! Help!", "");
        }

        public void ClosePopup()
        {
            PopupManager.Instance.RemoveActivePopup(this);
            OnClosed?.Invoke();
            PopupManager.Instance.CloseScenePopup(SceneNames.BANNED_ACCOUNT_POPUP);
            PopupManager.Instance.CloseAllPopups();
        }

        private void SendEmail(string email, string subject, string body)
        {
            Application.OpenURL($"mailto:{email}?subject={subject}&body={body}");
        }

        private void SetOptoutToggleActive(bool active)
        {
            optoutToggle.gameObject.SetActive(active);
        }

        private void HandleOptoutAgoraBanPopup(bool isOn)
        {
            OptoutAgoraBanPopup = isOn;
        }

        private void SetMessageDirty()
        {
            lblMessage.text = string.Format(messageFormat, banTypeString, causes, LiftTime);
        }

        private void Awake()
        {
            messageFormat = lblMessage.text;
            SetOptoutToggleActive(false);
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            optoutToggle.onValueChanged.AddListener(HandleOptoutAgoraBanPopup);
        }

        private void OnDisable()
        {
            optoutToggle.onValueChanged.RemoveListener(HandleOptoutAgoraBanPopup);
        }

        private void Start()
        {
            PopupManager.Instance.CloseAllPopups();
        }

        private void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            SetMessageDirty();
        }
    }
}
