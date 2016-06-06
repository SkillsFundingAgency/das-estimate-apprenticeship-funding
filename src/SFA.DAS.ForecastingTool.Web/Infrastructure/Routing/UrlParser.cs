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
                return ProcessEnglishFractionPath(parts);
            }
            if (parts.Length == 3)
            {
                return ProcessTrainingCoursePath(parts);
            }
            if (parts.Length == 4)
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
            if (result.IsErrored)
            {
                return result;
            }

            result.ActionName = "Paybill";
            return result;
        }
        private ParsedUrl ProcessEnglishFractionPath(string[] parts)
        {
            var result = ProcessPaybillPath(parts);
            if (result.IsErrored)
            {
                return result;
            }

            int paybill;
            if (!int.TryParse(parts[1], out paybill) || paybill <= 0)
            {
                result.IsErrored = true;
                result.RouteValues.Add("ErrorMessage", "Paybill is not a valid entry");
                result.ActionName = "Paybill";
            }
            else
            {
                result.ActionName = "EnglishFraction";
            }

            result.RouteValues.Add("Paybill", paybill);
            return result;
        }
        private ParsedUrl ProcessTrainingCoursePath(string[] parts)
        {
            var result = ProcessEnglishFractionPath(parts);
            if (result.IsErrored)
            {
                return result;
            }

            int englishFraction;
            if (!int.TryParse(parts[2], out englishFraction) || englishFraction <= 0 || englishFraction > 100)
            {
                result.IsErrored = true;
                result.RouteValues.Add("ErrorMessage", "English fraction is not a valid entry");
                result.ActionName = "EnglishFraction";
            }
            else
            {
                result.ActionName = "TrainingCourse";
            }

            result.RouteValues.Add("EnglishFraction", englishFraction);
            return result;
        }
        private ParsedUrl ProcessResultsPath(string[] parts)
        {
            var result = ProcessTrainingCoursePath(parts);
            if (result.IsErrored)
            {
                return result;
            }

            var splitPoint = parts[3].IndexOf('x');
            int standardQty;
            int standardCode;
            if (splitPoint < 1
                || !int.TryParse(parts[3].Substring(0, splitPoint), out standardQty)
                || !int.TryParse(parts[3].Substring(splitPoint + 1), out standardCode))
            {
                result.IsErrored = true;
                result.RouteValues.Add("ErrorMessage", "Number of apprentices or training standard invalid");
                result.ActionName = "TrainingCourse";
                return result;
            }

            if (standardCode > 0 && standardQty == 0)
            {
                result.IsErrored = true;
                result.RouteValues.Add("ErrorMessage", "Must have at least 1 apprentice to calculate. Alternatively you can skip this step");
                result.ActionName = "TrainingCourse";
                return result;
            }

            Standard standard = null;
            if (standardQty > 0 || standardCode > 0)
            {
                standard = _standardsRepository.GetByCodeAsync(standardCode).Result;
                if (standard == null)
                {
                    result.IsErrored = true;
                    result.RouteValues.Add("ErrorMessage", "Number of apprentices or training standard invalid");
                    result.ActionName = "TrainingCourse";
                    return result;
                }
            }

            result.ActionName = "Results";
            result.RouteValues.Add("SelectedStandard.Qty", standardQty);
            result.RouteValues.Add("SelectedStandard.Code", standardCode);
            result.RouteValues.Add("SelectedStandard.Name", standard?.Name);
            return result;
        }
    }
}