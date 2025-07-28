using mvcFirstApp.Models.Entities;
using mvcFirstApp.ViewModels;

namespace mvcFirstApp.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        List<CourseAllResultsViewModel> GetAllTraineeResults(int courseId);
    }
}
