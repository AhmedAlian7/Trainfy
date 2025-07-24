using mvcFirstApp.ViewModels;

namespace mvcFirstApp.Repositories
{
    public interface ITraineeRepository
    {
        TraineeAllResultsVM GetTraineeResult(int traineeId, int courseId);
        List<TraineeAllResultsVM> GetAllTraineeResults(int traineeId);
    }
}
