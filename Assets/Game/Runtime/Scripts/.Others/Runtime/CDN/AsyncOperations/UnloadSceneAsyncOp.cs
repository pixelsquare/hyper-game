#if ADDRESSABLES_ENABLED
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Kumu.Kulitan.CDN
{
    public class UnloadSceneAsyncOp : CDNAsyncOperation
    {
        public static UnloadSceneAsyncOp Create(SceneInstance sceneInstance) => new() { instance = sceneInstance };

        public override bool IsFailed => unloadSceneOp.Status == AsyncOperationStatus.Failed;

        public override object Result => result;

        public override float Progress
        {
            get
            {
                if (unloadSceneOp.IsValid())
                {
                    var progress = unloadSceneOp.PercentComplete;

                    if (progress >= 1f)
                    {
                        result = unloadSceneOp.Result;
                        Addressables.Release(unloadSceneOp);
                    }

                    base.Progress = progress;
                }

                return base.Progress;
            }
        }

        private SceneInstance instance;

        private object result = null;
        private AsyncOperationHandle unloadSceneOp;

        /// <summary>
        /// Prevents the creation of this class with `new` keyword.
        /// </summary>
        private UnloadSceneAsyncOp()
        {
        }

        public override void StartOp()
        {
            unloadSceneOp = Addressables.UnloadSceneAsync(instance, false);
        }
    }
}
#endif
