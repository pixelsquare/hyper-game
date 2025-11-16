using Kumu.Kulitan.Multiplayer;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class DisconnectTestTool : MonoBehaviour
    {
        [MenuItem("Simulation/Quantum Disconnect")]
        public static void SimulateDisconnect()
        {
            ConnectionManager.Client.Disconnect(DisconnectCause.None);
        }

        [MenuItem("Simulation/Client Timeout")]
        public static void SimulateClientTimeout()
        {
            ConnectionManager.Client.Disconnect(DisconnectCause.ClientTimeout);
        }
    }
}
