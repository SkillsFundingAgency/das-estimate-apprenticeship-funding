using System;

namespace SFA.DAS.ForecastingTool.Core.Models
{
    public class CohortModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public DateTime StartDate { get; set; }
    }
}