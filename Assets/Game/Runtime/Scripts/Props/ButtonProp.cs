using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(Button))]
    public class ButtonProp : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextProp text;

        public ReactiveProperty<string> Text => text.Text;
        public ReactiveProperty<Color> Color { get; set; }
        public ReactiveProperty<bool> IsInteractable { get; set; }
        public Subject<Unit> OnClickEvent { get; set; } = new();

        private void Awake()
        {
            if (button == null && !TryGetComponent(out button))
            {
                return;
            }

            Color = new(button.image.color);
            IsInteractable = new(button.interactable);
            var d = Disposable.CreateBuilder();
            button.OnClickAsObservable()
                .Subscribe(_ => OnClickEvent.OnNext(Unit.Default))
                .AddTo(ref d);
            Color
                .Subscribe(x => button.image.color = x)
                .AddTo(ref d);
            IsInteractable
                .Subscribe(x => button.interactable = x)
                .AddTo(ref d);
            d.RegisterTo(destroyCancellationToken);
        }
    }
}
