using DG.Tweening;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Kumu.Kulitan.Gifting
{
    public class VGGiftButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtGiftName;
        [SerializeField] private TextMeshProUGUI txtGiftPrice;
        [SerializeField] private TextMeshProUGUI txtSendPrompt;
        [SerializeField] private Image imgIcon;
        private VirtualGiftData giftData;
        private Button button;
        private VGComboController comboController;
        private Slot<string> eventSlot;
        private int counter;
        private Vector3 origSendPromptScale; 
        private AsyncOperationHandle<Sprite> spriteOperation;
        private Tweener punchTweener;
        
        public VirtualGiftData GiftData => giftData;
        
        public void SetDetails(VirtualGiftData giftData, VGComboController comboController)
        {
            this.giftData = giftData;
            txtGiftName.text = giftData.giftName;
            LoadIcon(this.giftData.giftId);
            this.comboController = comboController;
            txtGiftPrice.text = $"<sprite=1> {giftData.cost.amount}";
        }

        public void SetInteractable(bool isInteractable)
        {
            button.interactable = isInteractable;
        }

        private void OnClick()
        {
            comboController.QueueGift(giftData);
        }
        
        private void OnComboQueued(IEvent<string> obj)
        {
            var eventInfo = (VGComboQueuedEvent)obj;
            if (eventInfo.Id != giftData.giftId)
            {
                return;
            }

            punchTweener.Pause();
            txtSendPrompt.rectTransform.localScale = origSendPromptScale;
            counter++;
            txtSendPrompt.text = $"Sending... x{counter}";
            txtSendPrompt.gameObject.SetActive(true);
            punchTweener = txtSendPrompt.rectTransform.DOPunchScale(origSendPromptScale * 1.001f, 0.2f);
        }
        
        private void OnComboEnd(IEvent<string> obj)
        {
            counter = 0;
            txtSendPrompt.gameObject.SetActive(false);
        }

        private void LoadIcon(string giftId)
        {
            var giftConfig = VirtualGiftDatabase.Current.GetGift(giftId);
            spriteOperation = Addressables.LoadAssetAsync<Sprite>(giftConfig.SpriteRef);
            spriteOperation.Completed += OnIconLoaded;
        }
        
        private void UnloadIcon()
        {
            if (!spriteOperation.IsValid())
            {
                return;
            }

            Addressables.Release(spriteOperation);
        }

        private void OnIconLoaded(AsyncOperationHandle<Sprite> operationHandle)
        {
            imgIcon.sprite = operationHandle.Result;
        }

        private void Awake()
        {
            button = GetComponent<Button>();
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(VGComboQueuedEvent.EVENT_NAME,  OnComboQueued);
            eventSlot.SubscribeOn(VGComboEndEvent.EVENT_NAME,  OnComboEnd);
        }

        private void Start()
        {
            origSendPromptScale = txtSendPrompt.transform.localScale;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnDestroy()
        {
            UnloadIcon();
            eventSlot.Dispose();
        }
    }
}
