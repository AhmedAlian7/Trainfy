using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvcFirstApp.Models.Entities
{
    public class Trainee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        
        public string? Address { get; set; }

        public string? ImageUrl { get; set; }
        [Required(ErrorMessage = "Department is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a department.")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public ICollection<CourseResult>? CourseResults { get; set; } = new List<CourseResult>();

        // This property won't be mapped to database - used only for file upload
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
