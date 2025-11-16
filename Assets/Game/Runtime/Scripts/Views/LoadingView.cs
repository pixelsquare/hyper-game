using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LoadingView : View
    {
        [SerializeField] private Slider loadingSlider;

        public ReactiveProperty<float> Progress { get; set; } = new(0f);

        private void Awake()
        {
            var d = Disposable.CreateBuilder();
            Progress.Subscribe(x => loadingSlider.value = x)
                .AddTo(ref d);
            d.RegisterTo(destroyCancellationToken);
        }
    }
}
