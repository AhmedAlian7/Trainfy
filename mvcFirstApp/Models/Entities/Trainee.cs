namespace mvcFirstApp.Models.Entities
{
    public class Trainee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public ICollection<CourseResult>? CourseResults { get; set; } = new List<CourseResult>();
    }
}
