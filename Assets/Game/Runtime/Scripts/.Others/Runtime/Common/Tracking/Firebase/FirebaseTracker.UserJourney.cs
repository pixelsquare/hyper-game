using Firebase.Analytics;
using Kumu.Extensions;
using UnityEngine;

namespace Kumu.Kulitan.Tracking
{
    public partial class FirebaseTracker : IUserJourneyHandle
    {
        private const string USER_JOURNEY_EVENT_NAME = "user_journey";
        
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
            
            FirebaseAnalytics.LogEvent(USER_JOURNEY_EVENT_NAME, "step_id", paramName);
        }
    }
}
