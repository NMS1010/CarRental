using CarRental.Models.RentVehicle;

namespace CarRental.Repositories.RentVehicle
{
	public interface IRentVehicleRepository
	{
		Task RentVehicle(RentVehicleVM vm);
	}
}