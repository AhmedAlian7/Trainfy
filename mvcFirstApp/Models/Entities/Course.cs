using mvcFirstApp.CustomAttribute;

namespace mvcFirstApp.Models.Entities
{
    public class Course
    {
        public int Id { get; set; }
        [Unique]
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int Degree { get; set; }
        public int MinDegree { get; set; }
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public int InstructorId { get; set; }
        public Instructor? Instructor { get; set; }
        public ICollection<CourseResult>? CourseResults { get; set; } = new List<CourseResult>();
    }
}
