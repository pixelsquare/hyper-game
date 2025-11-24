using UnityEngine;
using R3;

namespace Game
{
    public class LoadingView : View
    {
        [SerializeField] private SliderProp loadingSlider;

        public ReactiveProperty<float> Progress => loadingSlider.Progress;
    }
}
