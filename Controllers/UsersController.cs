using CarRental.Repositories.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [Route("/admin/users")]
    [Authorize(Roles = "ADMIN")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAll();
            return View(users);
        }

        [Route("toggle/{id}")]
        public async Task<IActionResult> Toggle(long id)
        {
            var res = await _userRepository.Toggle(id);
            var url = "/admin/users" + (res ? "" : "?error=true");

            return Redirect(url);
        }

        [Route("{id}/approval-application")]
        public async Task<IActionResult> ApprovalApplication(long id)
        {
            var res = await _userRepository.GetById(id);

            return View("ApprovalApplication", res?.ApprovalApplication);
        }

        [Route("approve/{applicationId}")]
        public async Task<IActionResult> RequestApprove(long applicationId)
        {
            var res = await _userRepository.RequestApprove(applicationId);
            var url = "/admin/users" + (res ? "" : "?error=true");

            return Redirect(url);
        }

        [Route("evict/{applicationId}")]
        public async Task<IActionResult> RequestEvict(long applicationId)
        {
            var res = await _userRepository.RequestEvict(applicationId);
            var url = "/admin/users" + (res ? "" : "?error=true");

            return Redirect(url);
        }
    }
}