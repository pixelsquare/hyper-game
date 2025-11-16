using System;
using Firebase.Analytics;

namespace Kumu.Kulitan.Tracking
{
    public partial class FirebaseTracker : IScreenViewHandle
    {
        private string currentScreenId;
        private long screenViewTimestamp;
        
        public void OnScreenView(ScreenViewEvent eventData)
        {
            OnScreenEnter(eventData);
        }

        private void OnScreenEnter(ScreenViewEvent eventData)
        {
            TryOnScreenExit();
            currentScreenId = eventData.ScreenId;
            screenViewTimestamp = DateTime.Now.Ticks;
            
            var parameters = new Parameter[]
            { 
                new("screen_id", currentScreenId) 
            };

            FirebaseAnalytics.LogEvent("enter_screen", parameters);
        }

        private bool TryOnScreenExit()
        {
            if (string.IsNullOrEmpty(currentScreenId))
            {
                return false;
            }

            var duration = new TimeSpan(DateTime.Now.Ticks - screenViewTimestamp);
            
            var parameters = new Parameter[]
            {
                new("duration", duration.Seconds), 
                new("screen_id", currentScreenId) 
            };
            
            FirebaseAnalytics.LogEvent("exit_screen", parameters);
            return true;
        }
    }
}
