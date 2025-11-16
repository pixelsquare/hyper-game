using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Tracking
{
    public class ScreenViewEventSceneTrigger : MonoBehaviour
    {
        [SerializeField] private string screenId;
        [SerializeField] private bool triggerOnStart;
        
        public void Trigger()
        {
            Trigger(screenId);
        }

        public void Trigger(string screenId)
        {
            GlobalNotifier.Instance.Trigger(new ScreenViewEvent(screenId));
        }

        private void Start()
        {
            if (triggerOnStart)
            {
                Trigger();
            }
        }
    }
}
