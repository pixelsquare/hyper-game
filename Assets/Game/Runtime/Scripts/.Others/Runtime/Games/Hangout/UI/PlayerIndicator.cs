using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    /// <summary>
    /// Handles the PlayerIndicator for the game KumuJumper
    /// </summary>
    public class PlayerIndicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerText;
        [SerializeField] private Color localPlayerColor;
        [SerializeField] private Color otherPlayerColor;
        
        public void Initialize(string name, bool isLocal, Transform playerTransform, Camera camera)
        {
            playerText.color = isLocal ? localPlayerColor : otherPlayerColor;
            playerText.text = name;
        }
    }
}

