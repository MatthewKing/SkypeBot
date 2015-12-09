using System;

namespace SkypeBot
{
    public static class Extensions
    {
        public static bool ContainsIgnoreCase(this string target, string value)
        {
            return target.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
        }
    }
}
