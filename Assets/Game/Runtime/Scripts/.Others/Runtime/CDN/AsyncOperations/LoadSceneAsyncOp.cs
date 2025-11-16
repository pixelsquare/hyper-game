#if ADDRESSABLES_ENABLED
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.CDN
{
    public class LoadSceneAsyncOp : CDNAsyncOperation
    {
        public static LoadSceneAsyncOp Create(string scene, LoadSceneMode loadSceneMode, bool activateOnLoad)
        {
            return new()
            {
                key = scene,
                mode = loadSceneMode,
                activate = activateOnLoad
            };
        }

        public override bool IsFailed => loadSceneOp.Status == AsyncOperationStatus.Failed;

        public override object Result => result;

        public override float Progress
        {
            get
            {
                if (loadSceneOp.IsValid())
                {
                    var progress = loadSceneOp.PercentComplete;

                    if (progress >= 1f)
                    {
                        result = loadSceneOp.Result;
                    }

                    base.Progress = progress;
                }

                return base.Progress;
            }
        }

        private string key;
        private bool activate = true;
        private LoadSceneMode mode = LoadSceneMode.Single;

        private object result;
        private AsyncOperationHandle loadSceneOp;

        /// <summary>
        ///     Prevents the creation of this class with `new` keyword.
        /// </summary>
        private LoadSceneAsyncOp() { }

        public override void StartOp()
        {
            loadSceneOp = Addressables.LoadSceneAsync(key, mode, activate);
        }
    }
}
#endif
