using Kumu.Kulitan.Backend;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// UI element displaying Avatar Items via AvatarItem.Data.
    /// </summary>
    public class AvatarItem : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI costLabel;
        [SerializeField] private TextMeshProUGUI newLabel;
        [SerializeField] private Image currencyIcon;
        [SerializeField] private Image ownedIcon;
        [SerializeField] private Image itemIcon;
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private UnityEvent<bool> onItemOwned;
        [SerializeField] private UnityEvent<bool> onItemOwnedNot;
        [SerializeField] private UnityEvent<bool> onItemNew;
        [SerializeField] private UnityEvent<bool> onItemNewNot;
        [SerializeField] private Sprite[] currencyIconList; // 0 - diamond, 1 - coin
        
        private string avatarItemId;
        [HideInInspector] public AssetReferenceSprite wantedSprite;

        public string AvatarItemId => avatarItemId;

        #region Listener subscription
        public void AddListenerOnToggle(UnityAction<bool> onToggle)
        {
            toggle.onValueChanged.AddListener(onToggle);
        }

        public void RemoveListenerOnToggle(UnityAction<bool> onToggle)
        {
            toggle.onValueChanged.RemoveListener(onToggle);
        }

        public void ClearListeners()
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
        #endregion

        /// <summary>
        /// Populate display using given data. Specify useWantedRef override if there is a need to check
        /// a loaded sprite's GUID with specified wantedSprite's GUID. 
        /// </summary>
        public void Initialize(AvatarItemConfig itemConfig, bool useWantedRef = false)
        {
            label.text = itemConfig.Data.itemName;
            name = itemConfig.Data.itemId;
            avatarItemId = itemConfig.Data.itemId;
            SetCurrencyIcon(itemConfig.Data.cost);
            var isOwned = UserInventoryData.IsItemOwned(itemConfig.Data.itemId);
            costLabel.text = isOwned ? string.Empty : itemConfig.Data.cost.amount.ToString();

            LoadSprite(itemConfig.SpriteRef, useWantedRef);
            ToggleIsOwned(isOwned);
            gameObject.SetActive(true);
        }

        public void Deinitialize()
        {
            ClearListeners();

            itemIcon.sprite = null;
            itemIcon.enabled = false;
            itemIcon.name = "";

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Toggle with triggering event actions.
        /// </summary>
        /// <param name="isOn">True if the display is selected. Otherwise false.</param>
        public void Toggle(bool isOn)
        {
            toggle.isOn = isOn;
        }

        /// <summary>
        /// Toggle without triggering event actions.
        /// </summary>
        /// <param name="isOn">True if the display is selected. Otherwise false.</param>
        public void ToggleWithoutNotify(bool isOn)
        {
            toggle.SetIsOnWithoutNotify(isOn);
        }

        /// <summary>
        /// Toggle displaying icon notfying that the displayed item is owned.
        /// </summary>
        /// <param name="isOwned">True if the displayed item is owned.</param>
        public void ToggleIsOwned(bool isOwned)
        {
            onItemOwned.Invoke(isOwned);
            onItemOwnedNot.Invoke(!isOwned);
        }

        /// <summary>
        /// Toglge displaying label notifying that the displayed item is new.
        /// </summary>
        /// <param name="isNew">True if the displayed item is new.</param>
        public void ToggleIsNew(bool isNew)
        {
            onItemNew.Invoke(isNew);
            onItemNewNot.Invoke(!isNew);
        }

        /// <summary>
        /// Sets the currency icon to be used.
        /// Default is set to coin icon.
        /// </summary>
        /// <param name="currency">Currency data</param>
        private void SetCurrencyIcon(Currency currency)
        {
            if (!string.IsNullOrWhiteSpace(currency.code) && currency.code.Equals(Currency.UBE_DIA))
            {
                currencyIcon.sprite = currencyIconList[0];
                return;
            }

            currencyIcon.sprite = currencyIconList[1];
        }

        /// <summary>
        /// Loads the item preview image via Addressables.
        /// </summary>
        /// <param name="addressablePath">Path of the Addressable sprite to load.</param>
        private async void LoadSprite(AssetReferenceSprite spriteRef, bool useWantedRef = false)
        {
            itemIcon.enabled = false;
            
            loadingIndicator.SetActive(true);
            var sprite = await AvatarAddressablesUtility.LoadAddressable<Sprite>(spriteRef);
            if (loadingIndicator) // still alive?
            {
                loadingIndicator.SetActive(false);
            }

            if (sprite == null)
            {
                return;
            }
            
            if (useWantedRef && wantedSprite.AssetGUID != spriteRef.AssetGUID)
            {
                return;
            }
                
            if (itemIcon)
            {
                itemIcon.sprite = sprite;
                itemIcon.name = spriteRef.ToString();
                itemIcon.enabled = true;
            }
        }

        /// <summary>
        /// Shows the loading icon, used in scrollview optimization process.
        /// </summary>
        public void ShowLoadingIcon()
        {
            itemIcon.enabled = false;
            loadingIndicator.SetActive(true);
        }
        
        //discrete method for selection controller so it can also be referenced for removal
        public void OnSelectAction(AvatarItemConfig config, bool isSelected)
        {
            ToggleWithoutNotify(isSelected);
        }
    }
}
