using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    [RequireComponent(typeof(RinawaButton))]
    public class HideScreenButton : BaseButton
    {
        [SerializeField] private HudType _screenType;
        [Inject] private IHudManager _hudManager;

        protected override void OnButtonClicked()
        {
            _hudManager?.HideHud(_screenType);
        }
    }
}
