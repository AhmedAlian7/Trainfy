using Microsoft.EntityFrameworkCore;
using mvcFirstApp.Models.Data;
using mvcFirstApp.Models.Entities;
using mvcFirstApp.ViewModels;

namespace mvcFirstApp.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly AppDbContext _context;
        public CourseRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        List<CourseAllResultsViewModel> ICourseRepository.GetAllTraineeResults
            (int courseId, string searchTerm, string sortBy, string sortDirection, int minDegree, int maxDegree)
        {
            try
            {
                var course = _context.Courses
                    .Include(c => c.Department)
                    .Include(c => c.Instructor)
                    .FirstOrDefault(c => c.Id == courseId);

                if (course == null)
                    return new List<CourseAllResultsViewModel>();

                var courseResults = _context.CourseResults
                    .Include(cr => cr.Trainee)
                        .ThenInclude(t => t.Department)
                    .Where(cr => cr.CourseId == courseId)
                    .ToList();

                if (!courseResults.Any())
                    return new List<CourseAllResultsViewModel>();



                var query = courseResults.Select(cr => new CourseAllResultsViewModel
                {
                    Course = new CourseInfoViewModel
                    {
                        Id = course.Id,
                        Title = course.Title,
                        Credits = course.Credits,
                        Degree = course.Degree,
                        MinDegree = course.MinDegree,
                        DepartmentName = course.Department?.Name ?? "Unknown",
                        InstructorName = course.Instructor?.Name ?? "Unknown",
                        TotalEnrolled = courseResults.Count
                    },

                    Results = new List<TraineeResultViewModel>
                    {
                        new TraineeResultViewModel
                        {
                            TraineeId = cr.Trainee.Id,
                            Name = cr.Trainee.Name,
                            Email = cr.Trainee.Email,
                            ImageUrl = cr.Trainee.ImageUrl,
                            DepartmentName = cr.Trainee.Department?.Name ?? "Unknown",
                            Degree = cr.Degree,
                            GradeStatus = cr.Degree >= course.MinDegree ? "Pass" : "Fail",
                            GradeLetter = CalculateGradeLetter(cr.Degree, course.Degree),
                            DateOfBirth = cr.Trainee.DateOfBirth,
                            Age = CalculateAge(cr.Trainee.DateOfBirth)
                        }
                    },

                    // Empty statistics for individual records (will be calculated in controller if needed)
                    Statistics = new CourseStatisticsViewModel(),
                    SearchTerm = "",
                    SortBy = "Name",
                    SortDirection = "asc"
                }).AsQueryable();


                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(cr => cr.Results.Any(r => r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                                  r.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                }
                // Apply degree filter
                if (minDegree > 0 || maxDegree > 0)
                {
                    query = query.Where(cr => cr.Results.Any(r => r.Degree >= minDegree && r.Degree <= maxDegree));
                }
                // Apply sorting
                if (!string.IsNullOrEmpty(sortBy))
                {
                    if (sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                    {
                        query = sortBy switch
                        {
                            "Name" => query.OrderByDescending(cr => cr.Results.FirstOrDefault().Name),
                            "Degree" => query.OrderByDescending(cr => cr.Results.FirstOrDefault().Degree),
                            "Email" => query.OrderByDescending(cr => cr.Results.FirstOrDefault().Email),
                            "Department" => query.OrderByDescending(cr => cr.Results.FirstOrDefault().DepartmentName),
                            _ => query.OrderByDescending(cr => cr.Results.FirstOrDefault().Name)
                        };
                    }
                    else
                    {
                        query = sortBy switch
                        {
                            "Name" => query.OrderBy(cr => cr.Results.FirstOrDefault().Name),
                            "Degree" => query.OrderBy(cr => cr.Results.FirstOrDefault().Degree),
                            "Email" => query.OrderBy(cr => cr.Results.FirstOrDefault().Email),
                            "Department" => query.OrderByDescending(cr => cr.Results.FirstOrDefault().DepartmentName),
                            _ => query.OrderBy(cr => cr.Results.FirstOrDefault().Name)
                        };
                    }
                }

                // Convert to list and return
                var results = query.ToList();
                if (results.Count == 0)
                {
                    return new List<CourseAllResultsViewModel>();
                }
                return results;

            }
            catch (Exception ex)
            {
                return new List<CourseAllResultsViewModel>();
            }

        }

        private string CalculateGradeLetter(int degree, int maxDegree)
        {
            if (maxDegree == 0) return "N/A";

            double percentage = (double)degree / maxDegree * 100;

            return percentage switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C",
                >= 60 => "D",
                _ => "F"
            };
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age))
                age--;

            return age;
        }

    }
}
