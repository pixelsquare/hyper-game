using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class LegacySpawner : MonoBehaviour
    {
        private Transform _playerTransform;

        private DiContainer _diContainer;
        private LazyInject<UnitPlayer> _unitPlayer;

        [Inject]
        private void Construct(DiContainer diContainer, LazyInject<UnitPlayer> unitPlayer)
        {
            _diContainer = diContainer;
            _unitPlayer = unitPlayer;
        }

        private void OnSelectLegacy(IMessage message)
        {
            if (message.Data is not LegacyUpgradeSelection selection)
            {
                return;
            }

            if (selection._isNew)
            {
                var position = _playerTransform.position;
                var rotation = _playerTransform.rotation;
                _ = _diContainer.InstantiatePrefab(selection._prefab, position, rotation, _playerTransform) 
                     ?? Instantiate(selection._prefab, position, rotation, _playerTransform);
            }
            else
            {
                selection._legacy.LevelUp();
            }
        }

        private void OnGameStart(IMessage message)
        {
            _playerTransform = _unitPlayer.Value.transform;
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(GameEvents.AppState.OnGameStartEvent, OnGameStart, true);
            Dispatcher.AddListener(GameEvents.Gameplay.OnSelectLegacy, OnSelectLegacy, true);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(GameEvents.AppState.OnGameStartEvent, OnGameStart, true);
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnSelectLegacy, OnSelectLegacy, true);
        }
    }
}
