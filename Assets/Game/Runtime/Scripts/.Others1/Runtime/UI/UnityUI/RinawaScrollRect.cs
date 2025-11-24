using UnityEngine;
using UnityEngine.UI;

namespace Santelmo.Rinsurv
{
    [AddComponentMenu("Santelmo/UI/Rinawa Scroll Rect")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(RinawaImage))]
    public class RinawaScrollRect : ScrollRect
    {
    }
}
