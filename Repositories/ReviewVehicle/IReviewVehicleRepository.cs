using CarRental.Models.ReviewVehicle;

namespace CarRental.Repositories.ReviewVehicle
{
	public interface IReviewVehicleRepository
	{
		Task<IEnumerable<Entities.UserReviewVehicle>> GetAllReviewVehicles();

		Task Toggle(long id);

		Task AddReview(ReviewVehicleVM vm);
	}
}