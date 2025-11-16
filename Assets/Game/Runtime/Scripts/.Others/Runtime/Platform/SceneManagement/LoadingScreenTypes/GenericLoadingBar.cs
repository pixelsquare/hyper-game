using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class GenericLoadingBar : BaseLoadingScreen
    {
        [SerializeField] private SlicedFilledImage loadingBarFill;

        public override void UpdateLoadingProgress(float value)
        {
            var normalized = value / 100f;
            loadingBarFill.fillAmount = normalized;
        }
    }
}
