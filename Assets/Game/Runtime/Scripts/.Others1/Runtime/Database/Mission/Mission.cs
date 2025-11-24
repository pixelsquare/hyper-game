using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public class Mission : IAsset
    {
        [BoxGroup("Information", centerLabel: true)]
        [InlineButton("GenerateNewId", "Reset")]
        [SerializeField] private string _id;

        [BoxGroup("Information", centerLabel: true)]
        [SerializeField] private string _name;

        [BoxGroup("Information", centerLabel: true)]
        [MultiLineProperty(5)]
        [SerializeField] private string _description;

        [BoxGroup("Information", centerLabel: true)][HideLabel]
        [SerializeField] private MissionBiome _missionBiome;

        [BoxGroup("Level Details", centerLabel: true)]
        [SerializeField] private MissionLevelConfig[] _missionLevels;

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name => _name;
        public string Description => _description;
        public MissionBiome Biome => _missionBiome;
        public IEnumerable<MissionLevel> MissionLevels => _missionLevels.Select(x => x.Config);

        private void GenerateNewId()
        {
            _id = GameUtil.NewGuid();
        }
    }
}
