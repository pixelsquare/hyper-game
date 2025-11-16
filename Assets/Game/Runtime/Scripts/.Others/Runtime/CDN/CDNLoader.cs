using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;
using UnityEngine.Events;
#if ADDRESSABLES_ENABLED
using System;
using Kumu.Extensions;
using System.Collections;
using System.Threading.Tasks;
using Kumu.Kulitan.UI;
using UnityEngine.AddressableAssets;
#endif

namespace Kumu.Kulitan.CDN
{
    public class CDNLoader : MonoBehaviour
    {
        [SerializeField] private AssetLabelReference defaultLabelRef;

        public static event UnityAction OnInit;
        public static event UnityAction OnLoadStarted;
        public static event UnityAction OnLoadFinished;
        public static event UnityAction<float> OnLoadProgress;
        public static event UnityAction<string> OnLoadMessage;
        public bool IsLoading { get; private set; }

        public static event UnityAction OnDownloadStarted;
        public static event UnityAction OnDownloadFinished;
        public static event UnityAction<float> OnDownloadProgress;
        
        public bool IsDownloading { get; private set; }
        public long DownloadSize { get; private set; }
        public bool HasNewContent => DownloadSize > 0 || newCatalogsCount > 0;

        private int newCatalogsCount;

        public async void DownloadResources()
        {
            #if ADDRESSABLES_ENABLED
            try
            {
                // Need to check catalogs again
                // to resume downloading when interrupted.
                await CheckCatalogUpdates();

                IsDownloading = false;
                StopCoroutine(DownloadResourcesRoutine());
                StartCoroutine(DownloadResourcesRoutine());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                var popup = PopupManager.Instance.OpenErrorPopup("Error", "Downloading Failed! \nPlease try again.", "Ok");
                popup.OnClosed += DownloadResources;
            }
            #endif
        }

        public async void LoadResources()
        {
#if ADDRESSABLES_ENABLED
            try
            {
                // Need to check catalogs again
                // to resume downloading when interrupted.
                await CheckCatalogUpdates();

                IsLoading = false;
                StopCoroutine(LoadResourcesRoutine());
                StartCoroutine(LoadResourcesRoutine());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                var popup = PopupManager.Instance.OpenErrorPopup("Error", "Loading Failed! \nPlease try again.", "Ok");
                popup.OnClosed += LoadResources;
            }
#endif
        }

#if ADDRESSABLES_ENABLED
        private void OnEnable()
        {
            OnLoadFinished += HandleLoadFinished;
        }

        private void OnDisable()
        {
            OnLoadFinished -= HandleLoadFinished;
        }

        private void Start()
        {
            InitializeAddressables();
        }

        private async void InitializeAddressables()
        {
            try
            {
                await Addressables.InitializeAsync().Task;

                var downloadSizeOp = Addressables.GetDownloadSizeAsync(defaultLabelRef.labelString);
                DownloadSize = await downloadSizeOp.Task;
                Log($"Download size at {DownloadSize.ToSizeString()}");
                Addressables.Release(downloadSizeOp);

                await CheckCatalogUpdates();

                OnInit?.Invoke();
                Log("Initialization Done!");

                if (!HasNewContent)
                {
                    StartCoroutine(LoadQuantumAssets());
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                var popup = PopupManager.Instance.OpenErrorPopup("Fatal", "Failed to load assets.", "Exit");
                popup.OnClosed += ApplicationQuit;
            }
        }

        private IEnumerator DownloadResourcesRoutine()
        {
            if (IsDownloading)
            {
                LogError("Downloading in progress.");
                yield break;
            }
            
            Log("Downloading assets ...");
            
            IsDownloading = true;
            OnDownloadStarted?.Invoke();
            OnDownloadProgress?.Invoke(0.0f);

            var op = DownloadAssetsOp.Create(defaultLabelRef.labelString);
            op.StartOp();

            while (!op.IsDone)
            {
                OnLoadMessage?.Invoke($"Downloading... {((long)(DownloadSize * op.Progress)).ToSizeString()} / {DownloadSize.ToSizeString()}");
                OnDownloadProgress?.Invoke(Math.Max(0, op.Progress));
                yield return null;
            }
            
            if (op.IsFailed)
            {
                var popup = PopupManager.Instance.OpenErrorPopup("Error", "Downloading Failed! \nPlease try again.", "Ok");
                popup.OnClosed += DownloadResources;
                yield break;
            }
            
            OnDownloadProgress?.Invoke(op.Progress);
            yield return StartCoroutine(LoadResourcesRoutine());
            yield return new WaitForSeconds(1f);

            IsDownloading = false;
            OnDownloadFinished?.Invoke();
        }

        private IEnumerator LoadResourcesRoutine()
        {
            if (IsLoading)
            {
                LogError("Loading in progress.");
                yield break;
            }

            Log("Loading assets ...");

            IsLoading = true;
            OnLoadStarted?.Invoke();
            OnLoadProgress?.Invoke(0.0f);

            var op = LoadAssetsOp.Create(defaultLabelRef.labelString);
            op.StartOp();

            while (!op.IsDone)
            {
                OnLoadMessage?.Invoke($"Loading resources ... {((long)(DownloadSize * op.Progress)).ToSizeString()} / {DownloadSize.ToSizeString()}");
                OnLoadProgress?.Invoke(Math.Max(0, op.Progress));
                yield return null;
            }
            
            if (op.IsFailed)
            {
                var popup = PopupManager.Instance.OpenErrorPopup("Error", "Loading Failed! \nPlease try again.", "Ok");
                popup.OnClosed += LoadResources;
                yield break;
            }

            OnLoadProgress?.Invoke(op.Progress);
            yield return StartCoroutine(LoadQuantumAssets());
            yield return new WaitForSeconds(1f);

            IsLoading = false;
            OnLoadFinished?.Invoke();
        }

        private IEnumerator LoadQuantumAssets()
        {
            OnLoadProgress?.Invoke(0.0f);

            var op = LoadQuantumAssetsOp.Create();
            op.StartOp();

            while (!op.IsDone)
            {
                OnLoadMessage?.Invoke("Initializing ...");
                OnLoadProgress?.Invoke(Math.Max(0, op.Progress));
                yield return null;
            }
            
            if (op.IsFailed)
            {
                var popup = PopupManager.Instance.OpenErrorPopup("Error", "Failed to load assets. \nPlease try again.", "Ok");
                popup.OnClosed += LoadResources;
                yield break;
            }

            OnLoadProgress?.Invoke(Math.Max(0, op.Progress));
            yield return new WaitForSeconds(1f);
            OnLoadFinished?.Invoke();
        }

        private void HandleLoadFinished()
        {
            LoadMainScreen();
        }

        private async Task CheckCatalogUpdates()
        {
            Log("Checking for catalog updates.");
            var newCatalogs = await Addressables.CheckForCatalogUpdates().Task;
            newCatalogsCount = newCatalogs.Count;

            if (newCatalogsCount > 0)
            {
                Log($"Update found! Count: {newCatalogsCount}");
                await Addressables.UpdateCatalogs().Task;
            }
        }
#endif

        private void LoadMainScreen()
        {
            SceneLoadingManager.Instance.LoadMainScreenOnly(() =>
                    ConnectionManager.Instance.ConnectToServer(), false);
        }

        private void Log(string message)
        {
            $"ADDRESSABLES: {message}".Log();
        }
        
        private void LogError(string message)
        {
            $"ADDRESSABLES: {message}".LogError();
        }

        private void ApplicationQuit()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
        }
    }
}
