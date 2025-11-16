using System.Collections.Generic;
using UnityEngine;

namespace Kumu.Kulitan.ArcadePortal
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Game Data")]
    public class GameDataScriptableObject : ScriptableObject
    {
        [SerializeField] private List<GameData> gameData = new List<GameData>();
        public List<GameData> GameData => gameData;
    }
}