namespace mvcFirstApp.ViewModels
{
    public class TraineeAllResultsVM
    {
        public string TraineeName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int CourseCredits { get; set; }
        public string CourseDept { get; set; } = string.Empty;
        public int MaxGrade { get; set; }
        public int MinDegree { get; set; }
        public int Grade { get; set; }
        public bool IsPassed { get; set; }
        public decimal Percentage { get; set; }
        public string PerformanceLevel { get; set; } = string.Empty; // Excellent, Good, Fair, Poor

        // UI properties
        public string GradeDisplay => $"{Grade}/{MaxGrade} ({Percentage:F1}%)";
        public string PassedText => IsPassed ? "Passed" : "Failed";
        public string Color { get; set; } = string.Empty;


    }



}
