namespace SFA.DAS.ForecastingTool.Core.Models
{
    public class ApprenticeshipModel
    {
        public ApprenticeshipModel()
        {
        }
        public ApprenticeshipModel(Apprenticeship apprenticeship)
        {
            Code = apprenticeship.Code;
            Name = apprenticeship.Name;
            Price = apprenticeship.Price;
            Duration = apprenticeship.Duration;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Duration { get; set; }
    }
}