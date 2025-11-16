using System.Collections.Generic;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class MainPartSelection : MonoBehaviour
    {
        [SerializeField] private MainPartIcon activeIcon;
        [SerializeField] private List<MainPartIcon> icons = new List<MainPartIcon>();
        [SerializeField] private PartSelection partSelection;

        public void Initialize()
        {
            Select(icons[0]);
        }

        public void Select(MainPartIcon icon)
        {
            activeIcon = icon;
            partSelection.UpdateCollection(activeIcon.PartsCollection);
        }
    }
}
