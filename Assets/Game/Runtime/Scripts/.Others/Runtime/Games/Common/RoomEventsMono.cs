using Kumu.Kulitan.Events;
using Kumu.Kulitan.Tracking;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Common
{
    public class RoomEventsMono : MonoBehaviour
    {
        public UnityEvent onEnteredRoom;

        public UnityEvent onLeftRoom;

        private void OnLeaveHangoutEvent(IEvent<string> obj)
        {
            onLeftRoom.Invoke();
        }

        #region Monobehaviour

        private void Start()
        {
            onEnteredRoom.Invoke();
        }

        private void OnEnable()
        {
            GlobalNotifier.Instance.SubscribeOn(LeaveHangoutEvent.EVENT_ID, OnLeaveHangoutEvent);
        }

        private void OnDisable()
        {
            GlobalNotifier.Instance.UnSubscribeFor(LeaveHangoutEvent.EVENT_ID, OnLeaveHangoutEvent);
        }

        #endregion
    }
}
