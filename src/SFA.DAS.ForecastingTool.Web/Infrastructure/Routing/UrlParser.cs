using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            if (parts.Length == 1)
            {
                return ProcessPaybillPath(parts);
            }
            if (parts.Length == 2)
            {
                return ProcessEnglishFractionPath(parts);
            }
            if (parts.Length == 3 || parts.Length == 4)
            {
                return ProcessTrainingCoursePath(parts);
            }
            if (parts.Length == 5)
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

                ParseStandardsFromUrl(parts, result);
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
            
            int duration;
            if (!int.TryParse(parts.Length > 4 ? parts[4] : "12", out duration))
            {
                duration = 12;
            }
            duration = duration - (duration % 12);
            if (duration < 12)
            {
                duration = 12;
            }
            if (duration > 36)
            {
                duration = 36;
            }

            result.ActionName = "Results";
            result.RouteValues.Add("Duration", duration);

            return result;
        }

        private void ParseStandardsFromUrl(string[] parts, ParsedUrl result)
        {
            if (parts.Length < 4)
            {
                return;
            }

            var standards = parts[3].Split('_');
            
            for (var i = 0; i < standards.Length; i++)
            {
                var standardMatch = Regex.Match(standards[i], @"^(\d+)x(\d+)-(\d{4})-(\d{2})-(\d{2})$");
                DateTime standardStartDate;
                int standardCode;
                int standardQty;
                if (standardMatch.Success)
                {
                    standardQty = int.Parse(standardMatch.Groups[1].Value);
                    standardCode = int.Parse(standardMatch.Groups[2].Value);
                    try
                    {
                        standardStartDate = new DateTime(int.Parse(standardMatch.Groups[3].Value),
                            int.Parse(standardMatch.Groups[4].Value),
                            int.Parse(standardMatch.Groups[5].Value));
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        result.IsErrored = true;
                        result.RouteValues.Add("ErrorMessage", "Number of apprentices, training standard or start date invalid");
                        result.ActionName = "TrainingCourse";
                        return;
                    }
                }
                else
                {
                    result.IsErrored = true;
                    result.RouteValues.Add("ErrorMessage", "Number of apprentices, training standard or start date invalid");
                    result.ActionName = "TrainingCourse";
                    return;
                }

                if (standardCode > 0 && standardQty == 0)
                {
                    result.IsErrored = true;
                    result.RouteValues.Add("ErrorMessage",
                        "Must have at least 1 apprentice to calculate. Alternatively you can skip this step");
                    result.ActionName = "TrainingCourse";
                    return;
                }

                Standard standard = null;
                if (standardQty > 0 || standardCode > 0)
                {
                    standard = _standardsRepository.GetByCodeAsync(standardCode).Result;
                    if (standard == null)
                    {
                        result.IsErrored = true;
                        result.RouteValues.Add("ErrorMessage", "Number of apprentices, training standard or start date invalid");
                        result.ActionName = "TrainingCourse";
                        return;
                    }
                }


                result.RouteValues.Add($"SelectedStandards[{i}].Qty", standardQty);
                result.RouteValues.Add($"SelectedStandards[{i}].Code", standardCode);
                result.RouteValues.Add($"SelectedStandards[{i}].Name", standard?.Name);
                result.RouteValues.Add($"SelectedStandards[{i}].StartDate", standardStartDate);
            }
        }
    }
}