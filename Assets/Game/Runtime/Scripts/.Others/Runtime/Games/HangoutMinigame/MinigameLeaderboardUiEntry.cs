using Kumu.Kulitan.Common;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Minigame
{
    public class MinigameLeaderboardUiEntry : ListViewElement<MinigameLeaderboardEntry>
    {
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI rankLabel;

        public override void Refresh(MinigameLeaderboardEntry data)
        {
            nameLabel.text = $"{data.name}";
        }

        public void SetRankLabel(string label)
        {
            rankLabel.text = label;
        }
    }

    public struct MinigameLeaderboardEntry
    {
        public string name;
        public int score;
        public int rank;
    }
}
