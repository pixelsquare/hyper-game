using UnityEngine;
using TMPro;

namespace Kumu.Kulitan.Common
{
    public class GenericLoadingMessage : BaseLoadingScreen
    {
        [SerializeField] private TextMeshProUGUI loadingMsg;

        public void UpdateLoadingMessage(string msg)
        {
            loadingMsg.text = msg + "...";
        }

        public void SetMessageVisibility(bool enabled)
        {
            loadingMsg.gameObject.SetActive(enabled);
        }
    }
}
