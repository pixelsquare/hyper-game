using R3;
using TMPro;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextProp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public ReactiveProperty<string> Text { get; set; }

        private void Awake()
        {
            if (text == null && !TryGetComponent(out text))
            {
                return;
            }

            Text = new(text.text);
            var d = Disposable.CreateBuilder();
            Text
                .Subscribe(x => text.text = x)
                .AddTo(ref d);
            d.RegisterTo(destroyCancellationToken);
        }
    }
}
