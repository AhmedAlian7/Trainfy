namespace mvcFirstApp.ViewModels
{
    public class CourseStatisticsViewModel
    {
        public double AverageDegree { get; set; }
        public int HighestDegree { get; set; }
        public int LowestDegree { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public double PassPercentage { get; set; }
        public Dictionary<string, int> GradeDistribution { get; set; } = new();
    }



}
