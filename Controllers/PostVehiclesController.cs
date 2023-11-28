using CarRental.Models.PostVehicle;
using CarRental.Repositories.PostVehicle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [Route("/admin/post-vehicles")]
    [Authorize(Roles = "ADMIN")]
    public class PostVehiclesController : Controller
    {
        private readonly IPostVehicleRepository _postVehicleRepository;

        public PostVehiclesController(IPostVehicleRepository postVehicleRepository)
        {
            _postVehicleRepository = postVehicleRepository;
        }

        public async Task<ActionResult> Index()
        {
            var postVehicles = await _postVehicleRepository.GetPostVehicles();
            return View(postVehicles.ToList());
        }

        [Route("create")]
        public ActionResult Create()
        {
            return View(new PostVehicleVM());
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromForm] PostVehicleVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Dữ liệu không hợp lệ";
                return View(vm);
            }
            try
            {
                await _postVehicleRepository.AddPostVehicle(vm);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["Message"] = "Không thể tạo mới";
                return View(vm);
            }
        }

        [Route("edit/{id}")]
        public async Task<ActionResult> Edit(int id)
        {
            var postVehicle = await _postVehicleRepository.GetPostVehicle(id);
            ViewData["postVehicle"] = postVehicle;
            return View(new PostVehicleVM()
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

        [HttpPost("edit")]
        public async Task<ActionResult> Edit([FromForm] PostVehicleVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Dữ liệu không hợp lệ";
                return View(vm);
            }
            try
            {
                await _postVehicleRepository.UpdatePostVehicle(vm);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["Message"] = "Không thể tạo mới";
                return View(vm);
            }
        }

        [Route("delete/{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                await _postVehicleRepository.DeletePostVehicle(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Redirect("/admin/postVehicles?error=true");
            }
        }

        [Route("toggle/{id}")]
        public async Task<IActionResult> Toggle(long id)
        {
            await _postVehicleRepository.Toggle(id);

            return Redirect("/admin/post-vehicles");
        }
    }
}