using System;
using UnityEngine;

namespace Kumu.Kulitan.Lobby
{
    [CreateAssetMenu(fileName = "HangoutFilterConfig", menuName = "Kumu/Ube/Lobby/HangoutFilterConfig", order = 0)]
    public class HangoutFilterConfig : ScriptableObject
    {
        [SerializeField] private HangoutFilter[] filters;

        public HangoutFilter[] Filters => filters;

    }
    
    [Serializable]
    public struct HangoutFilter
    {
        public string id;
        public string label;
    }
}
