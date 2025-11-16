using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;

namespace Kumu.Kulitan.ArcadePortal
{
    public class GameSelection : MonoBehaviour
    {
        [SerializeField] private GameObject gameIconPrefab;
        [SerializeField] private RectTransform iconContainer;
        [SerializeField] private GameDataScriptableObject games;

        private List<GameSelectionIcon> gameIcons = new List<GameSelectionIcon>();

        [SerializeField] private UnityEvent<LevelConfigScriptableObject> onGameSelected;

        public void Initialize()
        {
            gameIcons.Clear();
            foreach (GameData g in games.GameData)
            {
                if (!g.IsActive)
                {
                    continue;
                }
                var icon = Instantiate(gameIconPrefab, iconContainer).GetComponent<GameSelectionIcon>();
                icon.Initialize(g, this);
                gameIcons.Add(icon);
            }
        }

        public void LoadGame(LevelConfigScriptableObject levelConfig)
        {
            GlobalNotifier.Instance.Trigger(new LoadMinigameEvent(levelConfig));
        }

        private void Start()
        {
            Initialize();
        }
    }    
}
