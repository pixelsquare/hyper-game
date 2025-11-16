using UnityEngine;

namespace Kumu.Kulitan.UI
{
    /// <summary>
    /// Handles any runtime nav button switching.
    /// </summary>
    public class UINavigationButtonSwitcher : MonoBehaviour
    {
        [SerializeField] private UINavigationButton[] buttons;
        [SerializeField] private int activeIndex = 0;

        public void SwitchButtons(int index)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i == index)
                {
                    buttons[i].gameObject.SetActive(true);
                }
                else
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
