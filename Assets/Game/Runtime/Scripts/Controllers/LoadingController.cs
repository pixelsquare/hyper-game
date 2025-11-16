using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Game
{
    public class LoadingController : Controller<LoadingView>
    {
        private void Start()
        {
            DOTween.To(() => view.Progress.Value, x => view.Progress.Value = x, 1f, 1f)
                .SetEase(Ease.Linear)
                .OnComplete(HandleLoadingComplete)
                .Play();
        }

        private void HandleLoadingComplete()
        {
            SceneManager.LoadScene("Title");
        }
    }
}
