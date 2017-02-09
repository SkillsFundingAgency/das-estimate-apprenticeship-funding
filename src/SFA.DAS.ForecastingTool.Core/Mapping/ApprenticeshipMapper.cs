﻿using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.ForecastingTool.Core.Models;

namespace SFA.DAS.ForecastingTool.Core.Mapping
{
    public class ApprenticeshipMapper
    {
        public Apprenticeship MapStandardToApprenticeship(StandardSummary standardSummary)
        {
            return new Apprenticeship
            {
                Code = standardSummary.Id,
                Duration = standardSummary.Duration,
                Price = standardSummary.MaxFunding,
                Name = standardSummary.Title
            };
        }

        public Apprenticeship MapFrameworkToApprenticeship(FrameworkSummary frameworkSummary)
        {
            return new Apprenticeship
            {
                Code = frameworkSummary.Id,
                Duration = frameworkSummary.Duration,
                Price = frameworkSummary.MaxFunding,
                Name = frameworkSummary.Title
            };
        }
    }
}