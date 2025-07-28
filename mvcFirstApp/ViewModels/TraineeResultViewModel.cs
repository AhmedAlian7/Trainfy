namespace mvcFirstApp.ViewModels
{
    public class TraineeResultViewModel
    {
        public int TraineeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int Degree { get; set; }
        public string GradeStatus { get; set; } = string.Empty; // Pass/Fail based on MinDegree
        public string GradeLetter { get; set; } = string.Empty; // A, B, C, D, F
        public DateTime? DateOfBirth { get; set; }
        public int Age { get; set; }
    }



}
