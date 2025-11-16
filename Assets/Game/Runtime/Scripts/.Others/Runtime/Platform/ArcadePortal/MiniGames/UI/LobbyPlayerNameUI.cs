using UnityEngine;
using TMPro;

namespace Kumu.Kulitan.Multiplayer
{
    public class LobbyPlayerNameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtPlayerName;
        [SerializeField] private Color localPlayerColor;
        [SerializeField] private Color otherPlayerColor;

        public void SetPlayerName(string name, bool isLocal)
        {
            txtPlayerName.color = isLocal ? localPlayerColor : otherPlayerColor;
            txtPlayerName.text = name;
        }
    }
}
