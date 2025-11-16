using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private int roundIntervalSec = 6;
        
        public int RoundIntervalSec => roundIntervalSec;
    }
}
