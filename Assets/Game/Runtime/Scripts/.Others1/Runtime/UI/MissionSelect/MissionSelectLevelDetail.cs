using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    public class MissionSelectLevelDetail : MonoBehaviour
    {
        [SerializeField] private RinawaText _missionLevelNameText;
        [SerializeField] private RinawaText _missionLevelDescText;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<string> _missionLevelNameProp = new(null);
        private readonly ReactiveProperty<string> _missionLevelDescProp = new(null);

        private void HandleActiveLevelSelected(IMessage message)
        {
            if (message.Data is not MissionLevel missionLevel)
            {
                return;
            }

            _missionLevelNameProp.Value = missionLevel.Name;
            _missionLevelDescProp.Value = missionLevel.Description;
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(UIEvent.OnActiveLevelSelected, HandleActiveLevelSelected, true);

            _missionLevelNameProp.Subscribe(x =>
            {
                _missionLevelNameText.text = x;
                _missionLevelNameText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _missionLevelDescProp.Subscribe(x =>
            {
                _missionLevelDescText.text = x;
                _missionLevelDescText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(UIEvent.OnActiveLevelSelected, HandleActiveLevelSelected, true);
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
