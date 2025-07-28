using Microsoft.EntityFrameworkCore;
using mvcFirstApp.Models.Data;
using mvcFirstApp.Models.Entities;
using mvcFirstApp.ViewModels;

namespace mvcFirstApp.Repositories
{
    public class TraineeRepository : Repository<Trainee>, ITraineeRepository
    {
        private readonly AppDbContext _context;

        public TraineeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public TraineeAllResultsVM GetTraineeResult(int traineeId, int courseId)
        {
            var courseRes = _context.CourseResults
                .Include(c => c.Trainee)
                .Include(c => c.Course)
                .FirstOrDefault(c => c.TraineeId == traineeId && c.CourseId == courseId);

            if (courseRes == null)
            {
                return null;
            }

            var passed = courseRes.Degree >= courseRes.Course.MinDegree;
            var percentage = courseRes.Course.Degree > 0
                ? (decimal)courseRes.Degree / courseRes.Course.Degree * 100
                : 0;

            var deptName = _context.Departments
                .Where(d => d.Id == courseRes.Course.DepartmentId)
                .Select(d => d.Name)
                .FirstOrDefault() ?? "N/A";

            return new TraineeAllResultsVM
            {
                TraineeName = courseRes.Trainee?.Name ?? "",
                CourseName = courseRes.Course.Title,
                CourseCredits = courseRes.Course.Credits,
                CourseDept = deptName,
                Grade = courseRes.Degree,
                MaxGrade = courseRes.Course.Degree,
                MinDegree = courseRes.Course.MinDegree,
                IsPassed = passed,
                Color = passed ? "green" : "red",
                Percentage = percentage,
                PerformanceLevel = GetPerformanceLevel(courseRes.Degree, courseRes.Course.MinDegree, courseRes.Course.Degree)
            };
        }

        public List<TraineeAllResultsVM> GetAllTraineeResults(int traineeId)
        {
            var results = _context.CourseResults
                .Include(c => c.Trainee)
                .Include(c => c.Course)
                .Where(c => c.TraineeId == traineeId)
                .ToList();

            if (results == null || results.Count == 0)
            {
                return new List<TraineeAllResultsVM>();
            }

            return results.Select(courseRes =>
            {
                var passed = courseRes.Degree >= courseRes.Course.MinDegree;
                var percentage = courseRes.Course.Degree > 0
                    ? (decimal)courseRes.Degree / courseRes.Course.Degree * 100
                    : 0;

                return new TraineeAllResultsVM
                {
                    TraineeName = courseRes.Trainee.Name,
                    CourseName = courseRes.Course.Title,
                    CourseId = courseRes.CourseId,
                    Grade = courseRes.Degree,
                    MaxGrade = courseRes.Course.Degree,
                    MinDegree = courseRes.Course.MinDegree,
                    IsPassed = passed,
                    Color = passed ? "green" : "red",
                    Percentage = percentage,
                    PerformanceLevel = GetPerformanceLevel(courseRes.Degree, courseRes.Course.MinDegree, courseRes.Course.Degree)
                };
            }).ToList();
        }

        public static string GetPerformanceLevel(int actualGrade, int minDegree, int maxDegree)
        {
            if (maxDegree <= minDegree) return "Invalid";
            if (actualGrade < minDegree) return "F"; // Failed

            var passingRange = maxDegree - minDegree;
            var gradeAboveMin = actualGrade - minDegree;
            var performancePercentage = (double)gradeAboveMin / passingRange;

            return performancePercentage switch
            {
                >= 0.85 => "A",
                >= 0.70 => "B",
                >= 0.5 => "C",
                >= 0.25 => "D",
                _ => "D-"
            };
        }
    }
}
    