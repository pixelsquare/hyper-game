using Kumu.Extensions;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : IEmoteUseHandle
    {
        public void OnEmoteUseEvent(EmoteUseEvent eventData)
        {
            $"<color=#{HEX}>using emote [{eventData.EmoteId}]</color>".Log();
        }
    }
}
