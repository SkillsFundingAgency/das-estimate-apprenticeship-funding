using System;
using System.Collections.Generic;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public static class UrlParser
    {
        public static ParsedUrl Parse(string url)
        {
            var parts = url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new ParsedUrl { RouteValues = new Dictionary<string, object>() };
            
            if (parts.Length == 1)
            {
                return ProcessPaybillPath(parts);
            }
            if (parts.Length == 2)
            {
                return ProcessTrainingCoursePath(parts);
            }
            if (parts.Length == 3)
            {
                return ProcessResultsPath(parts);
            }

            return ProcessWelcomePath(parts);
        }

        private static ParsedUrl ProcessWelcomePath(string[] parts)
        {
            return new ParsedUrl { ActionName = "Welcome", RouteValues = new Dictionary<string, object>() };
        }
        private static ParsedUrl ProcessPaybillPath(string[] parts)
        {
            var result = ProcessWelcomePath(parts);
            result.ActionName = "Paybill";
            return result;
        }
        private static ParsedUrl ProcessTrainingCoursePath(string[] parts)
        {
            var result = ProcessPaybillPath(parts);
            result.ActionName = "TrainingCourse";
            result.RouteValues.Add("Paybill", int.Parse(parts[1]));
            return result;
        }
        private static ParsedUrl ProcessResultsPath(string[] parts)
        {
            var result = ProcessTrainingCoursePath(parts);
            result.ActionName = "Results";
            result.RouteValues.Add("SelectedStandard.Qty", int.Parse(parts[2].Substring(0, 1)));
            result.RouteValues.Add("SelectedStandard.Code", int.Parse(parts[2].Substring(2)));
            return result;
        }
    }

    public class ParsedUrl
    {
        public string ActionName { get; set; }
        public Dictionary<string, object> RouteValues { get; set; }
    }
}