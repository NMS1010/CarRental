using CarRental.Models.PostVehicle;

namespace CarRental.Repositories.PostVehicle
{
    public interface IPostVehicleRepository
    {
        Task<IEnumerable<Entities.PostVehicle>> GetPostVehicles();
        Task<IEnumerable<Entities.PostVehicle>> GetPostVehiclesByUser(long userId);

        Task<IEnumerable<Entities.PostVehicle>> FindPostVehicles(string search);

        Task<Entities.PostVehicle> GetPostVehicle(long id);

        Task Toggle(long id);

        Task AddPostVehicle(PostVehicleVM pv);

        Task UpdatePostVehicle(PostVehicleVM pv);

        Task DeletePostVehicle(long id);
    }
}