using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AssetReferenceAttribute : PropertyAttribute
    {
        public Type Type { get; }

        public AssetReferenceAttribute(Type type)
        {
            Type = type;
        }
    }
}
