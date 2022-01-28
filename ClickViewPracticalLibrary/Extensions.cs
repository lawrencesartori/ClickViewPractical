using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickViewPracticalLibrary
{
    public static class Extensions
    {
        public static bool IsNullOrZero(this int? val)
        {
            return val == null || val <= 0;
        }

        public static bool IsNullOrEmpty(this string val)
        {
            return string.IsNullOrEmpty(val);
        }

        public static bool IsNullOrEmpty<T>(this List<T>? val)
        {
	        return val == null || !val.Any();
        }

        public static bool IsNotNullOrEmpty<T>(this List<T>? val)
        {
	        return !val.IsNullOrEmpty();
        }

        public static void LogError(this ILogger log, string message, string method)
        {
            log.LogError("{message} - within method {method}", message, method);
        }

        public static void LogError(this ILogger log, string message)
        {
            log.LogError(message,String.Empty);
        }
    }
}
