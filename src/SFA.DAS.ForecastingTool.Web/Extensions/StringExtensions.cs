using System;
using System.ComponentModel;

namespace SFA.DAS.ForecastingTool.Web.Extensions
{
    public static class StringExtensions
    {
        public static T? Convert<T>(this string input) where T : struct
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    // Cast ConvertFromString(string text) : object to (T)
                    return (T) converter.ConvertFromString(input);
                }
            }
            catch (NotSupportedException)
            {
                return null;
            }

            return null;
        }
    }
}