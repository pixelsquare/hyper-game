using System.Collections.Generic;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VGComboController : MonoBehaviour
    {
        [SerializeField] private float duration = 2f;
        private VGGiftListController listController;
        private float timer = 0;
        private bool isCounting = false;
        private Currency[] tempWalletBalance;
        private List<VirtualGiftGiftsData> giftsQueued = new();
        private string previousGiftId;
        
        public void QueueGift(VirtualGiftData vgData)
        {
            if (!string.IsNullOrWhiteSpace(previousGiftId) && 
                !string.Equals(vgData.giftId, previousGiftId))
            {
                EndCombo();
            }

            timer = 0;
            isCounting = true;
            previousGiftId = vgData.giftId;
            GlobalNotifier.Instance.Trigger(new VGComboQueuedEvent(previousGiftId));
            ParseVirtualGiftSendData(vgData);
        }

        private void EndCombo()
        {
            GlobalNotifier.Instance.Trigger(new VGComboEndEvent(previousGiftId));
            isCounting = false;
            timer = 0;
            previousGiftId = null;
            GlobalNotifier.Instance.Trigger(new VGSelectedEvent(giftsQueued.ToArray()));
            giftsQueued.Clear();
        }

        private void ParseVirtualGiftSendData(VirtualGiftData vgData)
        {
            var giftIdx = giftsQueued.FindIndex(data => data.id == vgData.giftId);
            if (giftIdx < 0)
            {
                var newGiftSendData = new VirtualGiftGiftsData()
                {
                    id = vgData.giftId,
                    quantity = 1
                };                    
                giftsQueued.Add(newGiftSendData);
                CrosscheckTempWalletBalance(vgData);
                return;
            }

            var newAmount = giftsQueued[giftIdx].quantity + 1;
            var modifiedData = new VirtualGiftGiftsData()
            {
                id = vgData.giftId,
                quantity = newAmount
            };

            giftsQueued[giftIdx] = modifiedData;
            $"{giftsQueued[giftIdx].id} {giftsQueued[giftIdx].quantity}".Log();
            CrosscheckTempWalletBalance(vgData);
        }

        /// <summary>
        /// Checks if user can still afford to spam the listed virtual gifts.
        /// </summary>
        private void CrosscheckTempWalletBalance(VirtualGiftData vgData)
        {
            var cost = vgData.cost;
            listController.DeductFromCachedWalletBalance(cost);
            listController.SetGiftAvailability();
        }

        private void Awake()
        {
            listController = FindObjectOfType<VGGiftListController>();
        }

        private void Update()
        {
            if (!isCounting)
            {
                return;
            }

            timer += Time.deltaTime;
            if (timer >= duration)
            {
                EndCombo();
            }
        }
    }
}
