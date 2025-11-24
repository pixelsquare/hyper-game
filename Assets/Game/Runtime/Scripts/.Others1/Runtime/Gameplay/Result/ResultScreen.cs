using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class ResultScreen : MonoBehaviour, IPointerDownHandler
    {
        public delegate void OnConfirmResult();
        public static event OnConfirmResult OnConfirmResultEvent;

        [Inject] private IMissionManager _missionManager;
        [Inject] private IInventoryModule _inventoryModule;

        public void OnPointerDown(PointerEventData eventData)
        {
            ToggleScreen(false);
            OnConfirmResultEvent?.Invoke();
            ProcessRewards();
        }

        public void ToggleScreen(bool isOn)
        {
            Time.timeScale = isOn ? 0f : 1f;
            gameObject.SetActive(isOn);
        }

        private void ProcessRewards()
        {
            var firstClearRewards = _missionManager.ActiveMissionLevel.FirstClearRewards;
            var otherRewards = _missionManager.ActiveMissionLevel.OtherRewards;

            // TODO: determine if first clear rewards are still valid to be rewarded
            foreach (var reward in firstClearRewards)
            {
                _inventoryModule.AddItem(reward.Id, reward.Amount);
            }

            foreach (var reward in otherRewards)
            {
                _inventoryModule.AddItem(reward.Id, reward.Amount);
            }
        }

        private void OnGameLose(IMessage message)
        {
            ToggleScreen(true);
        }

        private void OnGameWin(IMessage message)
        {
            ToggleScreen(true);
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(GameEvents.Gameplay.OnGameWin, OnGameWin);
            Dispatcher.AddListener(GameEvents.Gameplay.OnGameLose, OnGameLose);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnGameWin, OnGameWin, true);
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnGameLose, OnGameLose, true);
        }
    }
}
