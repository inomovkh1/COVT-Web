using Microsoft.AspNetCore.Mvc;

namespace COVT_Web.Controllers
{
    public class StatsionarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
