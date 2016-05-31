using System;
using System.Collections.Generic;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public class UrlParser
    {
        private readonly IStandardsRepository _standardsRepository;

        public UrlParser(IStandardsRepository standardsRepository)
        {
            _standardsRepository = standardsRepository;
        }

        public ParsedUrl Parse(string url)
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

        private ParsedUrl ProcessWelcomePath(string[] parts)
        {
            return new ParsedUrl { ActionName = "Welcome", RouteValues = new Dictionary<string, object>() };
        }
        private ParsedUrl ProcessPaybillPath(string[] parts)
        {
            var result = ProcessWelcomePath(parts);
            result.ActionName = "Paybill";
            return result;
        }
        private ParsedUrl ProcessTrainingCoursePath(string[] parts)
        {
            var result = ProcessPaybillPath(parts);
            result.ActionName = "TrainingCourse";
            result.RouteValues.Add("Paybill", int.Parse(parts[1]));
            return result;
        }
        private ParsedUrl ProcessResultsPath(string[] parts)
        {
            var result = ProcessTrainingCoursePath(parts);
            var standardQty = int.Parse(parts[2].Substring(0, 1));
            var standardCode = int.Parse(parts[2].Substring(2));
            var standard = _standardsRepository.GetByCodeAsync(standardCode).Result;

            result.ActionName = "Results";
            result.RouteValues.Add("SelectedStandard.Qty", standardQty);
            result.RouteValues.Add("SelectedStandard.Code", standardCode);
            result.RouteValues.Add("SelectedStandard.Name", standard?.Name);
            return result;
        }
    }
}