namespace mvcFirstApp.ViewModels
{
    public class CourseInfoViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int Degree { get; set; }
        public int MinDegree { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public int TotalEnrolled { get; set; }
    }



}
