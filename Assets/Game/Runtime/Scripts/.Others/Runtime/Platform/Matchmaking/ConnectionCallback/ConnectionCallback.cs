using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

namespace Kumu.Kulitan.Multiplayer
{
    public abstract class ConnectionCallback : MonoBehaviour, IConnectionCallbacks
    {
        public virtual void OnConnected() { }

        public virtual void OnConnectedToMaster() { }

        public virtual void OnCustomAuthenticationFailed(string debugMessage) { }

        public virtual void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }

        public virtual void OnDisconnected(DisconnectCause cause) { }

        public virtual void OnRegionListReceived(RegionHandler regionHandler) { }

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
