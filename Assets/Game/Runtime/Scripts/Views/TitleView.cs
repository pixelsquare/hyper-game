using R3;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class TitleView : View
    {
        [SerializeField] private Button playButton;

        public UnityAction OnPlayButtonClicked;

        private void Start()
        {
            var d = Disposable.CreateBuilder();
            playButton.onClick.AsObservable()
                .Subscribe(_ => OnPlayButtonClicked?.Invoke())
                .AddTo(ref d);
            d.RegisterTo(destroyCancellationToken);
        }
    }
}
