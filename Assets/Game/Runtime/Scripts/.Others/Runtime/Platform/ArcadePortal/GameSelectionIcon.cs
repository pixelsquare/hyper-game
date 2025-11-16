using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Kumu.Kulitan.Multiplayer;

namespace Kumu.Kulitan.ArcadePortal
{ 
    public class GameSelectionIcon : MonoBehaviour
    {
        private LevelConfigScriptableObject levelConfig;

        [SerializeField] private Image gameIcon;
        [SerializeField] private TMP_Text gameName;

        private GameSelection myGameSelection;

        public void Initialize(GameData gameData, GameSelection gameSelection)
        {
            levelConfig = gameData.LevelConfig;
            if (gameIcon)
            {
                gameIcon.sprite = gameData.Icon;
            }
            gameName.text = gameData.GameName;
            myGameSelection = gameSelection;
        }

        public void SelectGame()
        {
            myGameSelection.LoadGame(levelConfig);
        }
    }
}
