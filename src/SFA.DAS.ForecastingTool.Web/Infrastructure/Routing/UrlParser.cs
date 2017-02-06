using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SFA.DAS.ForecastingTool.Core.Models;
using SFA.DAS.ForecastingTool.Web.Infrastructure.Settings;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.Infrastructure.Routing
{
    public class UrlParser
    {
        private const long MaxPaybill = 512409557603043100;

        private readonly IStandardsRepository _standardsRepository;
        private readonly ICalculatorSettings _calculatorSettings;

        public UrlParser(IStandardsRepository standardsRepository, ICalculatorSettings calculatorSettings)
        {
            _standardsRepository = standardsRepository;
            _calculatorSettings = calculatorSettings;
        }

        public ParsedUrl Parse(string url, string queryString)
        {
            var parts = url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var queryParts = queryString.Split(new[] {'?', '&'}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return ProcessPaybillPath(parts, queryParts);
            }
            if (parts.Length == 2)
            {
                return ProcessEnglishFractionPath(parts, queryParts);
            }
            if (parts.Length == 3 || parts.Length == 4)
            {
                return ProcessTrainingCoursePath(parts, queryParts);
            }
            if (parts.Length == 5)
            {
                return ProcessResultsPath(parts, queryParts);
            }

            return ProcessWelcomePath(parts, queryParts);
        }

        private ParsedUrl ProcessWelcomePath(string[] parts, string[] queryParts)
        {
            return new ParsedUrl { ActionName = "Welcome", RouteValues = new Dictionary<string, object>() };
        }
        private ParsedUrl ProcessPaybillPath(string[] parts, string[] queryParts)
        {
            var result = ProcessWelcomePath(parts, queryParts);
            if (result.IsErrored)
            {
                return result;
            }

            if (parts.Length == 1)
            {
                var previousAnswer = GetPreviousAnswerValue(queryParts);
                long paybill;
                if (!string.IsNullOrEmpty(previousAnswer) && long.TryParse(previousAnswer, out paybill))
                {
                    result.RouteValues.Add("Paybill", paybill);
                }
            }

            result.ActionName = "Paybill";
            return result;
        }
        private ParsedUrl ProcessEnglishFractionPath(string[] parts, string[] queryParts)
        {
            var result = ProcessPaybillPath(parts, queryParts);
            if (result.IsErrored)
            {
                return result;
            }

            long paybill;
            if (!long.TryParse(parts[1], out paybill) || paybill <= 0 || paybill > MaxPaybill)
            {
                result.IsErrored = true;
                result.RouteValues.Add("ErrorMessage", "Enter the amount of your organisation’s UK payroll");
                result.ActionName = "Paybill";
            }
            else
            {
                if (parts.Length == 2)
                {
                    var previousAnswer = GetPreviousAnswerValue(queryParts);
                    int englishFraction;
                    if (!string.IsNullOrEmpty(previousAnswer) && int.TryParse(previousAnswer, out englishFraction))
                    {
                        result.RouteValues.Add("EnglishFraction", englishFraction);
                    }
                }

                result.ActionName = "EnglishFraction";
            }

            result.RouteValues.Add("Paybill", paybill);
            return result;
        }
        private ParsedUrl ProcessTrainingCoursePath(string[] parts, string[] queryParts)
        {
            var result = ProcessEnglishFractionPath(parts, queryParts);
            if (result.IsErrored)
            {
                return result;
            }

            int englishFraction = 0;
            if (parts[2].ToUpper() != "NA" && (!int.TryParse(parts[2], out englishFraction) || englishFraction <= 0 || englishFraction > 100))
            {
                result.IsErrored = true;
                result.RouteValues.Add("ErrorMessage", "English percentage is not a valid entry");
                result.ActionName = "EnglishFraction";
            }
            else
            {
                result.ActionName = "TrainingCourse";

                var previousAnswer = GetPreviousAnswerValue(queryParts);
                if (parts.Length >= 4)
                {
                    ParseStandardsFromUrl(parts[3], result);
                }
                else if (!string.IsNullOrEmpty(previousAnswer))
                {
                    ParseStandardsFromUrl(previousAnswer, result);
                }
            }

            result.RouteValues.Add("EnglishFraction", englishFraction);
            return result;
        }
        private ParsedUrl ProcessResultsPath(string[] parts, string[] queryParts)
        {
            var result = ProcessTrainingCoursePath(parts, queryParts);
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
            if (duration > _calculatorSettings.ForecastDuration)
            {
                duration = _calculatorSettings.ForecastDuration;
            }

            result.ActionName = "Results";
            result.RouteValues.Add("Duration", duration);

            return result;
        }

        private void ParseStandardsFromUrl(string standardsPart, ParsedUrl result)
        {
            var standards = standardsPart.Split('_');

            if (standards.Length == 1 && standards[0] == "0x0")
            {
                return;
            }

            var errorMessage = standards.Length > 1 
                                 ? "You need to choose at least one apprentice to add another row. Alternatively you can skip this step." 
                                 : "You must choose an apprenticeship training course. Alternatively you can skip this step.";
            
            for (var i = 0; i < standards.Length; i++)
            {
                var standardMatch = Regex.Match(standards[i], @"^(\d+)x([0-9-]+)x(\d{2})(\d{2})$");
                DateTime standardStartDate;
                string standardCode;
                int standardQty;
                if (standardMatch.Success)
                {
                    
                    standardCode = standardMatch.Groups[2].Value;
                    try
                    {
                        standardStartDate = new DateTime(2000 + int.Parse(standardMatch.Groups[4].Value),
                            int.Parse(standardMatch.Groups[3].Value),
                            1);
                        if (standardStartDate.Year < 2017)
                        {
                            throw new ArgumentOutOfRangeException($"Year {standardStartDate.Year} is before 2017");
                        }
                        if (standardStartDate.Year == 2017 && standardStartDate.Month < 5)
                        {
                            throw new ArgumentOutOfRangeException($"Date {standardStartDate} is before May 2017");
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        result.IsErrored = true;
                        result.RouteValues.Add("ErrorMessage", "Start date invalid, please enter a date from May 2017");
                        result.ActionName = "TrainingCourse";
                        return;
                    }
                }
                else
                {
                    result.IsErrored = true;
                    result.RouteValues.Add("ErrorMessage", errorMessage);
                    result.ActionName = "TrainingCourse";
                    return;
                }
                
                var parseResult = int.TryParse(standardMatch.Groups[1].Value, out standardQty);

                if (!parseResult || standardQty > 1000000)
                {
                    result.IsErrored = true;
                    result.RouteValues.Add("ErrorMessage", "We've limited the number of apprentices you can select to 1,000,000 so please enter a number below this.");
                    result.ActionName = "TrainingCourse";
                    return;
                }
                // TODO: review
                if (/*standardCode > 0 && */standardQty == 0)
                {
                    result.IsErrored = true;
                    result.RouteValues.Add("ErrorMessage", errorMessage);
                    result.ActionName = "TrainingCourse";
                    return;
                }

                Standard standard = null;
                // TODO: review
                if (standardQty > 0/* || standardCode > 0*/)
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


                result.RouteValues.Add($"SelectedCohorts[{i}].Qty", standardQty);
                result.RouteValues.Add($"SelectedCohorts[{i}].Code", standardCode);
                result.RouteValues.Add($"SelectedCohorts[{i}].Name", standard?.Name);
                result.RouteValues.Add($"SelectedCohorts[{i}].StartDate", standardStartDate);
            }
        }

        private string GetPreviousAnswerValue(string[] queryParts)
        {
            return queryParts.FirstOrDefault(q => q.ToLower().StartsWith("previousanswer="))?.Substring(15);
        }
    }
}