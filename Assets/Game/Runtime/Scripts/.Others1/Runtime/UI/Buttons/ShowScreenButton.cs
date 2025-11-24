using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    [RequireComponent(typeof(RinawaButton))]
    public class ShowScreenButton : BaseButton
    {
        [SerializeField] private HudType _screenType;
        [Inject] private IHudManager _hudManager;

        protected override void OnButtonClicked()
        {
            _hudManager?.ShowHud(_screenType);
        }
    }
}
