using UnityEngine;

namespace Kumu.Kulitan.UI
{
    /// <summary>
    /// Handles the navigation bar behavior
    /// </summary>
    public class UINavigationController : MonoBehaviour
    {
        [SerializeField] private UINavigationButton[] navButtons;
        [SerializeField] private UINavigationPanel[] navPanels;

        public void Initialize()
        {
        }

        /// <summary>
        /// Method called when a navigation button is pressed.
        /// </summary>
        /// <param name="activatedNavButton">passes the navigation button pressed</param>
        public void NavPanelActivated(UINavigationButton activatedNavButton)
        {
            foreach (UINavigationButton navButton in navButtons)
            {
                if (navButton == activatedNavButton)
                {
                    navButton.ActivateButton();
                }
                else
                {
                    navButton.DeactivateButton();
                }
            }

            foreach (UINavigationPanel navPanel in navPanels)
            {
                if (activatedNavButton.NavPanel == navPanel)
                {
                    navPanel.ActivatePanel();
                }
                else
                {
                    navPanel.DeactivatePanel();
                }
            }
        }

        private void Start()
        {
            Initialize();
        }
    }
}
