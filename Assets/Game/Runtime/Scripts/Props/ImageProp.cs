using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(Image))]
    public class ImageProp : MonoBehaviour
    {
        [SerializeField] private Image image;

        public ReactiveProperty<Color> Color { get; set; } = new();

        private void Awake()
        {
            if (image == null && !TryGetComponent(out image))
            {
                return;
            }

            Color = new(image.color);
            var d = Disposable.CreateBuilder();
            Color
                .Subscribe(x => image.color = x)
                .AddTo(ref d);
            d.RegisterTo(destroyCancellationToken);
        }
    }
}
