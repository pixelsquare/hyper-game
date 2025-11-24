using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    public class MissionSelectDetail : MonoBehaviour
    {
        [SerializeField] private RinawaText _missionNameText;
        [SerializeField] private RinawaText _missionDescText;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<string> _missionNameProp = new(null);
        private readonly ReactiveProperty<string> _missionDescProp = new(null);

        private void HandleActiveMissionSelected(IMessage message)
        {
            if (message.Data is not Mission mission)
            {
                return;
            }

            _missionNameProp.Value = mission.Name;
            _missionDescProp.Value = mission.Description;
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(UIEvent.OnActiveMissionSelected, HandleActiveMissionSelected, true);

            _missionNameProp.Subscribe(x =>
            {
                _missionNameText.text = x;
                _missionNameText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _missionDescProp.Subscribe(x =>
            {
                _missionDescText.text = x;
                _missionDescText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(UIEvent.OnActiveMissionSelected, HandleActiveMissionSelected, true);
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
