using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvcFirstApp.Models.Entities
{
    public class Instructor
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

        // This property won't be mapped to database - used only for file upload
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

    }
}
