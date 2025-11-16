using Kumu.Extensions;
using Kumu.Kulitan.Avatar;
using TMPro;
using UnityEngine;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftPanelBlocker : MonoBehaviour
    {
        [SerializeField] private GameObject blockerOverlay;

        [SerializeField] private TMP_Text blockerOverlayText;
        
        [SerializeField] private bool runOnStart = true;

        private bool IsVgSynced => VirtualGiftDatabase.Current.IsSynced;
        
        public bool IsInventoryInitialized => UserInventoryData.IsInitialized;
        
        public bool IsBalanceUpdated => UserInventoryData.IsBalanceUpdated;

        public void ShowBlockerIfApplicable()
        {
            if (!blockerOverlay)
            {
                "Missing reference to blocker overlay!".LogError();
                return;
            }
            
            blockerOverlay.SetActive(false);
            
            if (!IsVgSynced)
            {
                blockerOverlay.SetActive(true);
                blockerOverlayText.text = "Something went wrong trying to retrieve gifts prices. Sending gifts is disabled.";
                return;
            }

            if (!IsInventoryInitialized || !IsBalanceUpdated)
            {
                blockerOverlay.SetActive(true);
                blockerOverlayText.text = "Something went wrong syncing your inventory. Sending gifts is disabled.";
            }
        }

        private void Start()
        {
            if (!runOnStart)
            {
                return;
            }

            ShowBlockerIfApplicable();
        }
    }
}
