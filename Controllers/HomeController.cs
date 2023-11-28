using CarRental.Models.PostVehicle;
using CarRental.Models.RentVehicle;
using CarRental.Models.ReviewVehicle;
using CarRental.Models.User;
using CarRental.Repositories.FollowVehicle;
using CarRental.Repositories.PostVehicle;
using CarRental.Repositories.RentVehicle;
using CarRental.Repositories.ReviewVehicle;
using CarRental.Repositories.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [Route("/")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IPostVehicleRepository _postVehicleRepository;
        private readonly IRentVehicleRepository _rentVehicleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFollowVehicleRepository _followVehicleRepository;
        private readonly IReviewVehicleRepository _reviewVehicleRepository;

        public HomeController(IPostVehicleRepository postVehicleRepository, IRentVehicleRepository rentVehicleRepository,
            IUserRepository userRepository, IFollowVehicleRepository followVehicleRepository,
            IReviewVehicleRepository reviewVehicleRepository)
        {
            _postVehicleRepository = postVehicleRepository;
            _rentVehicleRepository = rentVehicleRepository;
            _userRepository = userRepository;
            _followVehicleRepository = followVehicleRepository;
            _reviewVehicleRepository = reviewVehicleRepository;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var vehicles = await _postVehicleRepository.GetPostVehicles();
            vehicles = vehicles.Where(x => x.Status);
            return View(vehicles.ToList());
        }

        #region ApprovalApplication
        [Route("doi-tac")]
        public IActionResult ApprovalApplication()
        {
            return View(new ApprovalApplicationVM());
        }

        [HttpPost("approve")]
        public async Task<IActionResult> CreateApprovalApplication(ApprovalApplicationVM vm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("ApprovalApplication", vm);
                await _userRepository.CreateApprovalApplication(vm);
                HttpContext.Session.SetString("IsApproving", "true");
                return Redirect("/doi-tac");
            }
            catch
            {
                ViewData["Message"] = "Đã có lỗi xảy ra, vui lòng thử lại sau";
                return View("ApprovalApplication", vm);
            }
        }
        #endregion

        #region PostVehicle

        [Route("bai-dang")]
        public async Task<IActionResult> GetPosts()
        {
            var res = long.TryParse(HttpContext.Session.GetString("UserId"), out long userId);
            if (!res)
                return Redirect("/auth/login");

            var resp = await _postVehicleRepository.GetPostVehiclesByUser(userId);
            return View("Post", resp);
        }

        [Route("bai-dang/danh-sach-dang-ky/{id}")]
        public async Task<ActionResult> GetListUserOrder(int id)
        {
            var res = await _postVehicleRepository.GetPostVehicle(id);

            return View("ListUserOrderVehicle", res.UserRentCars.ToList());
        }

        [Route("tao-bai-dang")]
        public ActionResult Create()
        {
            return View("CreatePost", new PostVehicleVM());
        }

        [HttpPost("tao-bai-dang")]
        public async Task<ActionResult> Create([FromForm] PostVehicleVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Dữ liệu không hợp lệ";
                return View("CreatePost", vm);
            }
            try
            {
                await _postVehicleRepository.AddPostVehicle(vm);

                return Redirect("/bai-dang");
            }
            catch
            {
                ViewData["Message"] = "Không thể tạo mới";
                return View("CreatePost", vm);
            }
        }

        [Route("chinh-sua-bai-dang/{id}")]
        public async Task<ActionResult> Edit(int id)
        {
            var postVehicle = await _postVehicleRepository.GetPostVehicle(id);
            ViewData["postVehicle"] = postVehicle;
            return View("EditPost", new PostVehicleVM()
            {
                Id = postVehicle.Id,
                Address = postVehicle.Address,
                Category = postVehicle.Category,
                Description = postVehicle.Description,
                PlaceId = postVehicle.PlaceId,
                Price = postVehicle.Price,
                Title = postVehicle.Title,
                VehicleFuel = postVehicle.VehicleFuel,
                VehicleName = postVehicle.VehicleName,
                VehicleSeat = postVehicle.VehicleSeat,
                VehicleType = postVehicle.VehicleType,
                VehicleYear = postVehicle.VehicleYear,
                Status = postVehicle.Status
            });
        }

        [HttpPost("chinh-sua-bai-dang")]
        public async Task<ActionResult> Edit([FromForm] PostVehicleVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Dữ liệu không hợp lệ";
                return View("EditPost", vm);
            }
            try
            {
                await _postVehicleRepository.UpdatePostVehicle(vm);
                return Redirect("/bai-dang");
            }
            catch
            {
                ViewData["Message"] = "Không thể tạo mới";
                return View("EditPost", vm);
            }
        }
        #endregion

        [Route("xe-da-dat")]
        public async Task<IActionResult> GetOrderList()
        {
            var res = long.TryParse(HttpContext.Session.GetString("UserId"), out long userId);
            if (!res)
                return Redirect("/auth/login");

            var user = await _userRepository.GetById(userId);

            return View("VehicleRent", user.UserRentVehicles.ToList());
        }

        #region FollowVehicle

        [Route("danh-sach-yeu-thich")]
        public async Task<IActionResult> GetFollowList()
        {

            var follows = await _followVehicleRepository.GetAllFollowVehicles();

            return View("FollowVehicle", follows);
        }

        [Route("theo-doi/{id}")]
        public async Task<IActionResult> Follow(long id)
        {
            await _followVehicleRepository.FollowVehicle(id);

            return Redirect("/danh-sach-yeu-thich");
        }

        [Route("bo-theo-doi/{id}")]
        public async Task<IActionResult> UnFollow(long id)
        {
            await _followVehicleRepository.UnfollowVehicle(id);

            return Redirect("/danh-sach-yeu-thich");
        }

        #endregion

        #region Account

        [Route("thong-tin-tai-khoan/chi-tiet")]
        public async Task<IActionResult> GetUser()
        {
            var res = long.TryParse(HttpContext.Session.GetString("UserId"), out long userId);
            if (!res)
                return Redirect("/auth/login");

            var user = await _userRepository.GetById(userId);

            return View("Account", user);
        }

        [HttpPost("edit-user")]
        public async Task<IActionResult> EditUser([FromForm] UserEditVM vm)
        {
            var res = long.TryParse(HttpContext.Session.GetString("UserId"), out long userId);
            if (!res)
                return Redirect("/auth/login");
            vm.Id = userId;
            try
            {
                await _userRepository.EditInfoUser(vm);
                return Redirect("/thong-tin-tai-khoan/chi-tiet?success=true");
            }
            catch
            {
                return Redirect("/thong-tin-tai-khoan/chi-tiet?error=true");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordVM vm)
        {
            var res = long.TryParse(HttpContext.Session.GetString("UserId"), out long userId);
            if (!res)
                return Redirect("/auth/login");
            vm.Id = userId;

            try
            {
                var isSuccess = await _userRepository.ChangePasswordUser(vm);
                return Redirect("/thong-tin-tai-khoan/chi-tiet?success=" + isSuccess);
            }
            catch
            {
                return Redirect("/thong-tin-tai-khoan/chi-tiet?error=true");
            }

        }

        #endregion

        [HttpPost("review")]
        public async Task<IActionResult> ReviewVehicle([FromForm] ReviewVehicleVM vm)
        {
            await _reviewVehicleRepository.AddReview(vm);

            return Redirect("/chi-tiet-xe/" + vm.PostVehicleId);
        }

        [Route("tim-xe")]
        [AllowAnonymous]
        public async Task<IActionResult> FindVehicle([FromQuery] string search)
        {
            var res = await _postVehicleRepository.FindPostVehicles(search);
            return View("Index", res);
        }

        [Route("chi-tiet-xe/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetailVehicle(long id)
        {
            var res = await _postVehicleRepository.GetPostVehicle(id);
            return View("PostVehicleDetail", res);
        }

        [Route("thong-tin-dat-xe/{id}")]
        public async Task<IActionResult> GetOrderVehicle(long idVehicle, DateTime startDate, DateTime endDate)
        {
            var res = await _postVehicleRepository.GetPostVehicle(idVehicle);
            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;

            return View(res);
        }

        [HttpPost("dat-xe")]
        public async Task<IActionResult> OrderVehicle([FromForm] RentVehicleVM vm)
        {
            await _rentVehicleRepository.RentVehicle(vm);

            return Redirect("/chi-tiet-xe/" + vm.PostVehicleId + "?success=true");
        }
    }
}