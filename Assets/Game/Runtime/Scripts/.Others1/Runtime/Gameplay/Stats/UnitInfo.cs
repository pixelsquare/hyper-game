using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public struct UnitInfo
    {
        [SerializeField] private string _name;

        [MultiLineProperty(5)]
        [SerializeField] private string _description;

        public string Name => _name;
        public string Description => _description;
    }
}
