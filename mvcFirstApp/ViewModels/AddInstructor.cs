using mvcFirstApp.Models.Entities;

namespace mvcFirstApp.ViewModels
{
    public class AddInstructor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public double Salary { get; set; }
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public ICollection<Course>? Courses { get; set; } = new List<Course>();

        public List<Department> SelectedDepartments { get; set; } = new List<Department>();
        public List<int> SelectedCourses { get; set; } = new List<int>();
    }

}
