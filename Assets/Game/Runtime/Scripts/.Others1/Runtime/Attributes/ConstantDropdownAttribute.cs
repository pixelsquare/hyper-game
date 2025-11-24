using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConstantDropdownAttribute : PropertyAttribute
    {
        public Type Type { get; }

        public ConstantDropdownAttribute(Type type)
        {
            Type = type;
        }
    }
}
