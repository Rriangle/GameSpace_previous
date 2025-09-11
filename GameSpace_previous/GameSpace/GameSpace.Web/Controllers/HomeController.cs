using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;

namespace GameSpace.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserReadOnlyRepository _userRepository;

        public HomeController(IUserReadOnlyRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userCount = await _userRepository.GetUserCountAsync();
                ViewBag.UserCount = userCount;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
