using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class BGMTrigger : MonoBehaviour
    {
        [SerializeField] private BGMHandler handler;
        [SerializeField] private int bgmIndex;

        private void OnTriggerEnter(Collider other)
        {
            if (handler == null)
            {
                Debug.LogWarning($"{gameObject.name} [BGMTrigger]: Handler object not specified");
                return;
            }

            var playerView = other.gameObject.GetComponentInParent<PlayerView>();

            if (playerView != null && QuantumRunner.Default.Game.PlayerIsLocal(playerView.PlayerRef))
            {
                handler.PlayTrack(bgmIndex);
            }
        }
    }
}
