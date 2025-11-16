using System;
using Kumu.Extensions;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : IScreenViewHandle
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
            $"<color=#{HEX}>entered {currentScreenId}</color>".Log();
        }

        private bool TryOnScreenExit()
        {
            if (string.IsNullOrEmpty(currentScreenId))
            {
                return false;
            }

            var duration = new TimeSpan(DateTime.Now.Ticks - photomodeTimestamp);
            $"<color=#{HEX}>exited {currentScreenId}, stayed {duration.Seconds} seconds</color>".Log();
            return true;
        }
    }
}
