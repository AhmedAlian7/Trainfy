using mvcFirstApp.Models.Entities;
using mvcFirstApp.ViewModels;

namespace mvcFirstApp.Repositories
{
    public interface ITraineeRepository : IRepository<Trainee>
    {
        TraineeAllResultsVM GetTraineeResult(int traineeId, int courseId);
        List<TraineeAllResultsVM> GetAllTraineeResults(int traineeId);
    }
}
