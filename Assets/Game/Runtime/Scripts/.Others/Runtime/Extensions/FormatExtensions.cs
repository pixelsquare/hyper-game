using System.Text.RegularExpressions;

namespace Kumu.Extensions
{
    public static class FormatExtensions
    {
        public static string CamelToSnakeCase(this string value)
        {
            return Regex.Replace(
                    value,
                    "[a-z][A-Z]",
                    m => m.ToString().ToLower().Insert(1, "_"))
                .ToLower();
        }
    }
}
