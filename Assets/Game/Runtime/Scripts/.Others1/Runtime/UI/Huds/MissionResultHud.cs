using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    using GameEvent = GameEvents.Gameplay;

    public class MissionResultHud : BaseHud
    {
        public enum ResultType
        {
            Quit,
            Lose,
            Win
        }

        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _losePanel;
        [SerializeField] private GameObject _quitPanel;

        [SerializeField] private MissionRewardPopulator _rewardPopulator;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<ResultType> _resultTypeProp = new(ResultType.Quit);

        public MissionResultHud Setup(ResultType resultType, IEnumerable<IItem> rewardItems = null)
        {
            _resultTypeProp.Value = resultType;
            _rewardPopulator.Setup(rewardItems);
            Task = UniTask.WaitUntil(() => Input.GetMouseButtonUp(0));
            return this;
        }

        private void OnEnable()
        {
            _resultTypeProp.Subscribe(x =>
            {
                _winPanel.SetActive(x == ResultType.Win);
                _losePanel.SetActive(x == ResultType.Lose);
                _quitPanel.SetActive(x == ResultType.Quit);
            }).AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
