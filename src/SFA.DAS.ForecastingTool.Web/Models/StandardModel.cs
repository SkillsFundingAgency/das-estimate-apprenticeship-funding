using System;
using SFA.DAS.ForecastingTool.Web.Standards;

namespace SFA.DAS.ForecastingTool.Web.Models
{
    public class StandardModel
    {
        public StandardModel()
        {
        }
        public StandardModel(Standard standard)
        {
            Code = standard.Code;
            Name = standard.Name;
            Price = standard.Price;
            Duration = standard.Duration;
        }

        public int Code { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Duration { get; set; }
    }
}