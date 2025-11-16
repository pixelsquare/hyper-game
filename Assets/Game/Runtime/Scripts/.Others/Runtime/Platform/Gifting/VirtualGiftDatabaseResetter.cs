using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    /// <summary>
    /// Helps fix a issue with VirtualGiftDatabase where IsSynced
    /// value persists across sessions of Unity in play mode.
    /// </summary>
    public class VirtualGiftDatabaseResetter : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            VirtualGiftDatabase.Current.IsSynced = false;
        }
    }
}
