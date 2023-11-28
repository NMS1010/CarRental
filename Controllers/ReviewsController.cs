using CarRental.Repositories.ReviewVehicle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [Route("/admin/reviews")]
    [Authorize(Roles = "ADMIN")]
    public class ReviewsController : Controller
    {
        private readonly IReviewVehicleRepository _reviewVehicleRepository;

        public ReviewsController(IReviewVehicleRepository reviewVehicleRepository)
        {
            _reviewVehicleRepository = reviewVehicleRepository;
        }

        public async Task<IActionResult> Index()
        {
            var res = await _reviewVehicleRepository.GetAllReviewVehicles();

            return View(res);
        }

        [Route("toggle/{id}")]
        public async Task<IActionResult> Toggle(long id)
        {
            await _reviewVehicleRepository.Toggle(id);

            return Redirect("/admin/reviews");
        }
    }
}