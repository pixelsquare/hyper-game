using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class ActiveTabSelector : MonoBehaviour
    {
        [Serializable]
        public struct ToggleInfo
        {
            public Toggle toggle;
            public TMP_Text toggleText;
        }

        [SerializeField] private ToggleInfo[] toggles;

        public void HighlightActiveToggle(Toggle toggle)
        {
            foreach (var info in toggles)
            {
                var isToggleActive = info.toggle == toggle;
                info.toggleText.fontStyle = isToggleActive ? FontStyles.Underline : FontStyles.Normal;
            }
        }
    }
}
