using System;
using Kumu.Extensions;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : IPhotoModeHandle
    {
        private long photomodeTimestamp;
        private int photomodeAmount;
        
        public void OnPhotoModeStart(IEvent<string> eventData)
        {
            photomodeTimestamp = DateTime.Now.Ticks;
            photomodeAmount = 0;
        }

        public void OnPhotoTaken(IEvent<string> eventData)
        {
            photomodeAmount++;
        }

        public void OnPhotoModeEnd(IEvent<string> eventData)
        {
            var now = DateTime.Now.Ticks;
            var duration = new TimeSpan(now - photomodeTimestamp);
            $"<color=#{HEX}>tracking photo mode {duration.Seconds} seconds, {photomodeAmount} photos</color>".Log();            
        }
    }
}
