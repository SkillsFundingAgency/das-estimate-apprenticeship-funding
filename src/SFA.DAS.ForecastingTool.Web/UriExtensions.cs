using System;
using System.Linq;

namespace SFA.DAS.ForecastingTool.Web
{
    public static class UriExtensions
    {
        public static string GetUrlToSegment(this Uri url, int segments)
        {
            var result = url.Segments.Take(segments + 1).Aggregate((x, y) => x + y);
            return result.EndsWith("/") ? result : result + "/";
        }
    }
}