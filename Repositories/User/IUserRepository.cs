using CarRental.Models.Auth;
using CarRental.Models.User;

namespace CarRental.Repositories.User
{
    public interface IUserRepository
    {
        Task<bool> Register(RegisterVM request);

        Task<Entities.User> Login(LoginVM request);

        Task<Entities.User> GetById(long id);

        Task<bool> Toggle(long id);

        Task<bool> RequestApprove(long id);

        Task<bool> RequestEvict(long id);

        Task CreateApprovalApplication(ApprovalApplicationVM vm);

        Task<List<Entities.User>> GetAll();

        Task EditInfoUser(UserEditVM vm);

        Task<bool> ChangePasswordUser(ChangePasswordVM vm);
    }
}