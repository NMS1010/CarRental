using CarRental.Common.Enums;
using CarRental.Entities;
using CarRental.Models.Auth;
using CarRental.Repositories.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRental.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Route("login")]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return Redirect("/");
            }
            return View();
        }

        private async Task AssignCookies(User user)
        {
            var claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(ClaimTypes.Name, user.Name)
            };

            var identity = new ClaimsIdentity(claims, "UserAuth");

            var userPrincipal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties()
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(10),
            };
            await HttpContext.SignInAsync(
                "UserAuth",
                userPrincipal,
                authProperties
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var user = await _userRepository.Login(vm);
            if (user == null)
            {
                ViewData["Message"] = "Tài khoản hoặc mật khẩu không chính xác";
                return View(vm);
            }

            //var obj = JsonConvert.SerializeObject(user);
            //HttpContext.Session.SetString("User", obj);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Role", user.Role.ToString());
            HttpContext.Session.SetString("Name", user.Name.ToString());
            HttpContext.Session.SetString("Avatar", user.Avatar.ToString());
            HttpContext.Session.SetString("IsApproving", (user.ApprovalApplication != null && !user.ApprovalApplication.IsApprove).ToString());
            HttpContext.Session.SetString("IsApproved", (user.ApprovalApplication != null && user.ApprovalApplication.IsApprove).ToString());
            await AssignCookies(user);

            if (user.Role == ROLE_TYPE.ADMIN)
            {
                return Redirect("/admin/users");
            }
            return Redirect("/");
        }

        [Route("register")]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return Redirect("/");
            }
            return View(new RegisterVM());
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var success = await _userRepository.Register(vm);
            if (!success)
            {
                ViewData["Message"] = "Thông tin không hợp lệ, tên tài khoản hoặc email đã tồn tại";
                return View(vm);
            }

            return Redirect("/auth/login?success=true");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("User");
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("Avatar");
            HttpContext.Session.Remove("Name");
            await HttpContext.SignOutAsync("UserAuth");

            return Redirect("/");
        }
    }
}