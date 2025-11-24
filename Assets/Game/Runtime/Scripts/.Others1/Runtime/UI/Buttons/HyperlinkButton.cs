using UnityEngine;

namespace Santelmo.Rinsurv
{
    [RequireComponent(typeof(RinawaButton))]
    public class HyperlinkButton : BaseButton
    {
        [SerializeField] private string _url;

        protected override void OnButtonClicked()
        {
            Application.OpenURL(_url);
        }
    }
}
