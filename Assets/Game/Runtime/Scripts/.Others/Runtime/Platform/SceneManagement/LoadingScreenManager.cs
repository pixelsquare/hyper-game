using UnityEngine;
using Kumu.Extensions;

namespace Kumu.Kulitan.Common
{
    public class LoadingScreenManager : SingletonMonoBehaviour<LoadingScreenManager>
    {
        [SerializeField] private RectTransform screenContainer;
        [SerializeField] private SerializableDictionary<LoadingScreenType, GameObject> loadingScreenMapping;
        [SerializeField] private GameObject loadingOverlay;

        private ILoadingScreen currentLoadingScreen = null;

        /// <summary>
        /// Manually shows a loading screen on top of any UI
        /// No scene loading 
        /// </summary>
        /// <param name="screenType">type of loading screen to use</param>
        /// <returns>loading screen</returns>
        public ILoadingScreen ShowLoadingScreen(LoadingScreenType screenType)
        {
            // if a loading screen is open, skip opening another
            if (currentLoadingScreen != null)
            {
                return currentLoadingScreen;
            }

            currentLoadingScreen = CreateLoadingScreen(screenType);
            return currentLoadingScreen;
        }

        public void HideLoadingScreen()
        {
            if (currentLoadingScreen == null)
            {
                "Trying to hide loading screen but no screen is detected.".LogWarning();
                return;
            }
            currentLoadingScreen.Hide();
            currentLoadingScreen = null;
        }

        public void UpdateCurrentLoadingProgress(float loadingProgress)
        {
            if (currentLoadingScreen == null)
            {
                return;
            }

            var loadingScreen = currentLoadingScreen;
            loadingScreen.UpdateLoadingProgress(loadingProgress);
        }

        public void ShowHideLoadingOverlay(bool show)
        {
            loadingOverlay.SetActive(show);
        }

        private ILoadingScreen CreateLoadingScreen(LoadingScreenType screenType)
        {
            if (loadingScreenMapping.TryGetValue(screenType, out var screenToSpawn))
            {
                var screenInstance = Instantiate(screenToSpawn, screenContainer);
                var loadingScreen = screenInstance.GetComponent(typeof(ILoadingScreen)) as ILoadingScreen;
                loadingScreen.Show();
                return loadingScreen;
            }
            "Screen Type does not exist.".LogError();
            return null;
        }
    }

    public enum LoadingScreenType
    {
        GenericLoadingBar,
        GenericLoadingMessage,
    }
}
