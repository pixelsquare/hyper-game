using System;

namespace Santelmo.Rinsurv
{
    public static class EnumExtensions
    {
        public static string ToStringValue(this Enum value)
        {
            var enumValue = value.ToString();
            var enumType = value.GetType();
            var fieldInfo = enumType.GetField(enumValue);

            if (!Attribute.IsDefined(fieldInfo, typeof(StringValueAttribute)))
            {
                throw new Exception("Enum does not have StringValueAttribute.");
            }

            var attributes = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            return attributes?.Length > 0 ? attributes[0].Value : enumValue;
        }
    }
}
