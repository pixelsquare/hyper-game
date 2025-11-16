using System;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Tracking.Components
{
    public class UserJourneyEventSceneTrigger : MonoBehaviour
    {
        [SerializeField] private bool triggerOnStart;
        [SerializeField] private UserJourneyEvent.Checkpoint journeyCheckpoint;

        public void Trigger()
        {
            GlobalNotifier.Instance.Trigger(new UserJourneyEvent(journeyCheckpoint));
        }

        public void Trigger(string enumName)
        {
            if (Enum.TryParse(enumName, out UserJourneyEvent.Checkpoint enumValue)
                && Enum.IsDefined(typeof(UserJourneyEvent.Checkpoint), enumValue))
            {
                GlobalNotifier.Instance.Trigger(new UserJourneyEvent(enumValue));
            }
            else
            {
                $"{enumName} is not a valid value for UserjourneyEvent.Checkpoint".LogError();
            }
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
