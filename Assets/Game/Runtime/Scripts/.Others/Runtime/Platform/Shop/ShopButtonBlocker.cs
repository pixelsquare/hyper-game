using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class ShopButtonBlocker : MonoBehaviour
    {
        [SerializeField] private Button shopButton;

        [SerializeField] private Image shopBlocker;

        [SerializeField] private bool runOnStart = true;

        public void UpdateBlockerState()
        {
            var isUnblocked = UserInventoryData.IsInitialized && UserInventoryData.IsBalanceUpdated && ShopData.IsInitialized;
            
            shopButton.interactable = isUnblocked;
            shopBlocker.gameObject.SetActive(!isUnblocked);
        }

        private void HandleOnValuesUpdated()
        {
            UpdateBlockerState();
        }

        private void Start()
        {
            if (!runOnStart)
            {
                return;
            }
            
            UpdateBlockerState();
        }

        private void OnEnable()
        {
            UserInventoryData.OnValuesUpdated += HandleOnValuesUpdated;
            ShopData.OnValuesUpdated += HandleOnValuesUpdated;
        }

        private void OnDisable()
        {
            UserInventoryData.OnValuesUpdated -= HandleOnValuesUpdated;
            ShopData.OnValuesUpdated -= HandleOnValuesUpdated;
        }
    }
}
