using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class PhotoModeTutorial : MonoBehaviour
    {
        [SerializeField] private GameObject[] pages;

        private int currentPage;

        public void StartTutorial()
        {
            gameObject.SetActive(true);
            currentPage = 0;
            ShowPage(currentPage);
        }

        private void EndTutorial()
        {
            gameObject.SetActive(false);
            PlayerPrefs.SetInt(PhotoModeManager.TutorialPrefsKey, 1);
        }

        public void ShowNextPage()
        {
            var wantedPage = currentPage + 1;

            if (wantedPage < pages.Length)
            {
                currentPage = wantedPage;
                ShowPage(currentPage);
            }
            else
            {
                EndTutorial();
            }
        }

        private void ShowPage(int index)
        {
            for (var i=0; i < pages.Length; i++)
            {
                if (i == index)
                {
                    pages[i].SetActive(true);
                }
                else
                {
                    pages[i].SetActive(false);
                }
            }
        }
    }
}
