using System.Collections.Generic;
using Kumu.Extensions;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VGGiftListController : MonoBehaviour
    {
        [SerializeField] private VirtualGiftData.VGType giftListVgType = VirtualGiftData.VGType.Individual;
        [SerializeField] private List<VGGiftButton> vgListButtons = new();
        [SerializeField] private VGGiftButton giftButton;
        [SerializeField] private RectTransform buttonContainer;
        [SerializeField] private VGComboController comboController;
        [SerializeField] private GameObject giftListBlocker;
        [SerializeField] private bool initOnStart;
        
        // private int diamondCache = 0;
        // private int coinCache = 0;
        private Slot<string> eventSlot;
        
        /// <summary>
        /// A "cloned" wallet of UserInventoryData.UserWallet. All changes to coin balance for virtual gifting
        /// (deductions due to spending and additions due to refunding) are done on this wallet instead of the main one. 
        /// </summary>
        private UserWallet WalletCache { get; set; } = UserWallet.Default;

        public void Initialize()
        {
            LoadGiftList();
            SetGiftAvailability(true);
        }
        
        public void SetGiftListBlocker(bool isEnabled)
        {
            giftListBlocker.SetActive(isEnabled);
        }

        public void SetGiftAvailability(bool getWalletBalance = false)
        {
            foreach (var btn in vgListButtons)
            {
                var playerCount = VGPlayerListController.allPlayersGifteeData.Count;

                // if there are no other players, disable gifting
                if (playerCount <= 0)
                {
                    btn.SetInteractable(false);
                    continue;
                }

                btn.SetInteractable(WalletCache[btn.GiftData.cost.code] >= btn.GiftData.cost.amount);
            }
        }
        
        public void DeductFromCachedWalletBalance(Currency currency)
        {
            WalletCache[currency.code] -= currency.amount;
        }

        public void RefundToCachedWalletBalance(IEnumerable<Currency> currencies)
        {
            foreach (var c in currencies)
            {
                WalletCache[c.code] += c.amount;
            }
        }

        private void LoadGiftList()
        {
            ClearDisplay();
            foreach (var vg in VirtualGiftDatabase.Current.GiftConfigs)
            {
                if (vg.IsHidden)
                {
                    continue;
                }

                if (giftListVgType != vg.Data.giftType)
                {
                    continue;
                }

                var btn = Instantiate(giftButton, buttonContainer);
                btn.SetDetails(vg.Data, comboController);
                vgListButtons.Add(btn);
            }
        }

        private void ClearDisplay()
        {
            foreach (var btn in vgListButtons)
            {
                Destroy(btn.gameObject);
            }
            vgListButtons.Clear();
        }
        
        private void OnVgTypeChanged(IEvent<string> callback)
        {
            Initialize();
        }

        #region Monobehaviour

        private void Awake()
        {
            WalletCache = new UserWallet(UserInventoryData.UserWallet.GetCurrenciesAsArray());
        }

        private void Start()
        {
            if (initOnStart)
            {
                Initialize();
            }
        }

        private void OnEnable()
        { 
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(VGTypeSelectedEvent.EVENT_NAME, OnVgTypeChanged);
        }
 
        private void OnDisable()
        {
            eventSlot.Dispose();
        }

        #endregion
    }
}
