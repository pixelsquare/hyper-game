using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class BGMSwitchClick : MonoBehaviour
    {
        [SerializeField] private BGMHandler handler;
        [SerializeField] private int bgmIndex;

        public void ChangeBGM()
        {
            if (handler == null)
            {
                Debug.LogWarning($"{gameObject.name} [BGMTrigger]: Handler object not specified");
                return;
            }

            handler.PlayTrack(bgmIndex);
        }
    }
}
