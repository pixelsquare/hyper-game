using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class InGamePauseHud : BaseHud
    {
        [Inject] private IHudManager _hudManager;

        private void OnEnable()
        {
            if (_hudManager.HudExist(HudType.LegacyPicker))
            {
                return;
            }

            Time.timeScale = 0f;
        }

        private void OnDisable()
        {
            if (_hudManager.HudExist(HudType.LegacyPicker))
            {
                return;
            }

            Time.timeScale = 1f;
        }
    }
}
