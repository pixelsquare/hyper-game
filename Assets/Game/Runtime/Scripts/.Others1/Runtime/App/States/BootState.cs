using Cysharp.Threading.Tasks;
using Zenject;

namespace Santelmo.Rinsurv
{
    using SceneName = GameConstants.SceneNames;

    public class BootState : State
    {
        [Inject] private ISceneManager _sceneManager;

        public override void OnEnter()
        {
            EndState();
        }

        public override async void OnExit()
        {
            await UniTask.WaitUntil(() => _sceneManager.IsSceneLoaded(SceneName.AssetLoaderScene));
            _sceneManager.UnloadSceneAsync(SceneName.BootScene);
        }
    }
}
