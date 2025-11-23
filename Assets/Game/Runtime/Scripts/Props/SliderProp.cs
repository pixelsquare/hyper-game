using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class SliderProp : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        
        public ReactiveProperty<float> Progress { get; set; }
        public ReactiveProperty<(float min, float max)> MinMax { get; set; }

        private void Awake()
        {
            if (slider == null && !TryGetComponent(out slider))
            {
                return;
            }

            Progress = new(slider.value);
            MinMax = new((slider.minValue, slider.maxValue));
            var d = Disposable.CreateBuilder();
            Progress.Subscribe(x => slider.value = x)
                .AddTo(ref d);
            MinMax.Subscribe(x =>
            {
                slider.minValue = x.min;
                slider.maxValue = x.max;
            }).AddTo(ref d);
            d.RegisterTo(destroyCancellationToken);
        }
    }
}
