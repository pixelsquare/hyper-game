using System.Linq;
using UnityEngine;

namespace Kumu.Kulitan.Multiplayer
{
    /// <summary>
    /// Used by the RoomLayoutPopup object.
    /// This houses the list of room layout configs available to use in game.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Room Layouts Config")]
    public class RoomLayoutConfigs : ScriptableObject
    {
        [SerializeField] private RoomLayoutConfig[] roomLayoutConfigs;

        public RoomLayoutConfig[] LayoutConfigs
        {
            get => roomLayoutConfigs;
            set => roomLayoutConfigs = value;
        }

#if UNITY_EDITOR
        public void AddToConfig(RoomLayoutConfig config)
        {
            var configsList = roomLayoutConfigs.ToList();
            configsList.Add(config);
            roomLayoutConfigs = configsList.ToArray();
        }
#endif
    }
}
