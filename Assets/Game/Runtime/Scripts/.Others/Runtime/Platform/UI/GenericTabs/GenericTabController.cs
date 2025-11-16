using System.Collections.Generic;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public class GenericTabController : MonoBehaviour
    {
        [SerializeField] private List<GenericTabButton> tabButtons;
        [SerializeField] private List<GenericTabContent> tabContents;

        private GenericTabButton activeTab;

        public void UpdateActiveTab(GenericTabButton button)
        {
            for (var i = 0; i < tabButtons.Count; i++)
            {
                tabButtons[i].Initialize(this);
                if (tabButtons[i] != button)
                {
                    tabButtons[i].SetActive(false);
                    tabContents[i].SetContentActive(false);
                    tabContents[i].gameObject.SetActive(false);
                    continue;
                }
                
                tabButtons[i].SetActive(true);
                tabContents[i].gameObject.SetActive(true);
                tabContents[i].SetContentActive(true);
            }
        }

        public void Initialize()
        {
            foreach (var t in tabButtons)
            {
                t.Initialize(this);
            }
            UpdateActiveTab(tabButtons[0]);
        }
    }
}
