using Kumu.Extensions;
using UnityEngine;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : IUserJourneyHandle
    {
        public void OnUserJourneyEvent(UserJourneyEvent eventData)
        {
            var journeyLabel = eventData.Journey.ToString();
            var paramName = journeyLabel.CamelToSnakeCase();
            var prefsKey = $"journey_{paramName}";
            
            if (PlayerPrefs.HasKey(prefsKey))
            {
                return;
            }
            
            PlayerPrefs.SetInt(prefsKey, 1);

            $"<color=#{HEX}>first time {paramName}</color>".Log();
        }
    }
}
