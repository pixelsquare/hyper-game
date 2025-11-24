using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class ResultScreenManager : MonoBehaviour
    {
        [SerializeField] private ResultScreen _winScreen;
        [SerializeField] private ResultScreen _loseScreen;
        [SerializeField] private ResultScreen _quitScreen;

        private void OnGameQuit(IMessage message)
        {
            _quitScreen.ToggleScreen(true);
        }

        private void OnGameLose(IMessage message)
        {
            _loseScreen.ToggleScreen(true);
        }

        private void OnGameWin(IMessage message)
        {
            _winScreen.ToggleScreen(true);
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(GameEvents.Gameplay.OnGameWin, OnGameWin);
            Dispatcher.AddListener(GameEvents.Gameplay.OnGameLose, OnGameLose);
            Dispatcher.AddListener(GameEvents.Gameplay.OnGameQuit, OnGameQuit);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnGameWin, OnGameWin, true);
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnGameLose, OnGameLose, true);
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnGameQuit, OnGameQuit, true);
        }
    }
}
