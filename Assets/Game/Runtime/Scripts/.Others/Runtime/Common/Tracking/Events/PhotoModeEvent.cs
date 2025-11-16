using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public class PhotoModeEvent : Event<string>
    {
        public const string TAKE_ID = "PhotoModeTakeEvent";
        public const string END_ID = "PhotoModeEndEvent";

        public PhotoModeEvent() : base(END_ID)
        {
        }
    }

    public interface IPhotoModeHandle
    {
        public void OnPhotoModeStart(IEvent<string> eventData);
        public void OnPhotoTaken(IEvent<string> eventData);
        public void OnPhotoModeEnd(IEvent<string> eventData);
    }
}
