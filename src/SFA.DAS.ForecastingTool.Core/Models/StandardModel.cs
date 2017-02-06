namespace SFA.DAS.ForecastingTool.Core.Models
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

        public string Code { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Duration { get; set; }
    }
}