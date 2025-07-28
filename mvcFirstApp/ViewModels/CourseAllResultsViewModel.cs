namespace mvcFirstApp.ViewModels
{
    public class CourseAllResultsViewModel
    {
        public CourseInfoViewModel Course { get; set; } = new();
        public List<TraineeResultViewModel> Results { get; set; } = new();
        public CourseStatisticsViewModel Statistics { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string SortBy { get; set; } = "Name"; // Name, Degree, Email
        public string SortDirection { get; set; } = "asc"; // asc, desc
        public int? MinDegree { get; set; }
        public int? MaxDegree { get; set; }
    }
}
