using System.Collections.Generic;
using Photon.Realtime;
using Kumu.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Multiplayer
{
    public abstract class BaseLobbyHandler : MonoBehaviour, ILobbyCallbacks
    {
        [SerializeField] private UnityEvent onLobbyJoined;
        [SerializeField] private UnityEvent onLobbyLeft;

        public virtual void JoinLobby()
        {
            ConnectionManager.Client.OpJoinLobby(TypedLobby.Default);
        }

        #region LobbyCallbacks

        public virtual void OnJoinedLobby()
        {
            onLobbyJoined?.Invoke();
        }

        public virtual void OnLeftLobby()
        {
            onLobbyLeft?.Invoke();
        }

        public virtual void OnRoomListUpdate(List<RoomInfo> roomList)
        {
        }
        
        public virtual void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics) {}
        #endregion
        
        protected virtual void AddCallbackTarget()
        {
            ConnectionManager.Client.AddCallbackTarget(this);
        }
        
        protected virtual void RemoveCallbackTarget()
        {
            ConnectionManager.Client.RemoveCallbackTarget(this);
        }

        protected virtual void OnEnable()
        {
            AddCallbackTarget();
        }

        protected virtual void OnDisable()
        {
            RemoveCallbackTarget();
        }
    }
}
