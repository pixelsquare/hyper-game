using System;
using UnityEngine;
using Kumu.Kulitan.Multiplayer;

namespace Kumu.Kulitan.ArcadePortal
{
    [Serializable]
    public class GameData
    {
        [SerializeField] private string gameId = "kumujumper";
        [SerializeField] private string gameName = "Kumu Jumper";
        [SerializeField] private bool isActive = false;
        [SerializeField] private Sprite icon;
        [SerializeField] private LevelConfigScriptableObject levelConfig;

        public string GameId => gameId;
        public string GameName => gameName;
        public bool IsActive => isActive; 
        public Sprite Icon => icon;
        public LevelConfigScriptableObject LevelConfig => levelConfig;
    }
}
