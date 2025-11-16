using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Lobby
{
    public class LobbySortFilterTrigger : MonoBehaviour
    {
        public void OnConfirm()
        {
            GlobalNotifier.Instance.Trigger(new SortFilterRoomEvent());
        }
    }
}
