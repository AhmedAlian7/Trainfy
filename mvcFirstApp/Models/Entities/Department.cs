namespace mvcFirstApp.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public ICollection<Course>? Courses { get; set; } = new List<Course>();
        public ICollection<Instructor>? Instructors { get; set; } = new List<Instructor>();
        public ICollection<Trainee>? Trainees { get; set; } = new List<Trainee>();

    }
}
