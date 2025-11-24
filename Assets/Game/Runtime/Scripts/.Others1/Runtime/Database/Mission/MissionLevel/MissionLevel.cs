using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public class MissionLevel : IAsset
    {
        [BoxGroup("Information", centerLabel: true)]
        [InlineButton("GenerateNewId", "Reset")]
        [SerializeField] private string _id;

        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] private string _name;

        [BoxGroup("Information", centerLabel: true)]
        [MultiLineProperty(5)]
        [SerializeField] private string _description;

        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] private string _alias;

        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] private int _energyCost;

        [BoxGroup("Reward Detail", centerLabel: true)]
        [SerializeField] private MissionRewardConfig[] _firstClearRewards;

        [BoxGroup("Reward Detail", centerLabel: true)]
        [SerializeField] private MissionRewardConfig[] _otherRewards;

        [BoxGroup("Game Level Detail", centerLabel: true)][HideLabel]
        [SerializeField] private MissionGameLevel _gameLevel;

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name => _name;
        public string Description => _description;
        public string Alias => _alias;
        public int EnergyCost => _energyCost;

        public IEnumerable<MissionReward> FirstClearRewards => _firstClearRewards.Select(x => x.Config).OfType<MissionReward>();
        public IEnumerable<MissionReward> OtherRewards => _otherRewards.Select(x => x.Config).OfType<MissionReward>();
        public MissionGameLevel GameLevel => _gameLevel;

        private void GenerateNewId()
        {
            _id = GameUtil.NewGuid();
        }
    }
}
