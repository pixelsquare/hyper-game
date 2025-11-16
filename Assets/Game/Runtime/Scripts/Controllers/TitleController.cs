using UnityEngine.SceneManagement;

namespace Game
{
    public class TitleController : Controller<TitleView>
    {
        private void OnEnable()
        {
            view.OnPlayButtonClicked += HandlePlayButtonClicked;
        }

        private void OnDisable()
        {
            view.OnPlayButtonClicked -= HandlePlayButtonClicked;
        }

        private void HandlePlayButtonClicked()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
