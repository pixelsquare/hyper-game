using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv.Test
{
    public class GameplayInit : MonoBehaviour
    {
        [SerializeField] private GameObject _worldContext;
        [SerializeField] private GameObject[] _sceneObjects;

        private Stack<GameObject> _instances;
        private IMissionManager _missionManager;
        private DiContainer _container;

        [Inject]
        public void Construct(DiContainer container, IMissionManager missionManager)
        {
            _container = container;
            _missionManager = missionManager;
        }

        private async void OnLoadGame(IMessage message)
        {
            var environmentPrefab = _missionManager.ActiveMissionLevel.GameLevel._environment;
            _instances.Push(_container.InstantiatePrefab(environmentPrefab));

            var worldContext = _container.InstantiatePrefab(_worldContext);
            _instances.Push(worldContext);

            await UniTask.NextFrame();

            await GameplayUtility.InitializeGameplaySystemsAsync(worldContext);
            
            foreach (var sceneObject in _sceneObjects)
            {
                sceneObject.SetActive(true);
            }

            Dispatcher.SendMessage(GameEvents.AppState.OnLoadGameEndedEvent);
        }

        private void OnUnloadGame(IMessage message)
        {
            while (_instances.TryPop(out var instance))
            {
                Destroy(instance.gameObject);
            }

            foreach (var sceneObject in _sceneObjects)
            {
                sceneObject.SetActive(false);
            }
        }

        private void OnConfirmResult()
        {
            Dispatcher.SendMessage(GameEvents.AppState.OnGameEndedEvent);
        }

        private void Awake()
        {
            _instances = new Stack<GameObject>();
        }

        private void OnEnable()
        {
            ResultScreen.OnConfirmResultEvent += OnConfirmResult;
            Dispatcher.AddListener(GameEvents.AppState.OnLoadGameplayObjectsEvent, OnLoadGame);
            Dispatcher.AddListener(GameEvents.AppState.OnUnloadGameStartEvent, OnUnloadGame);
        }

        private void OnDisable()
        {
            ResultScreen.OnConfirmResultEvent -= OnConfirmResult;
            Dispatcher.RemoveListener(GameEvents.AppState.OnLoadGameplayObjectsEvent, OnLoadGame, true);
            Dispatcher.RemoveListener(GameEvents.AppState.OnUnloadGameStartEvent, OnUnloadGame, true);
        }
    }
}
