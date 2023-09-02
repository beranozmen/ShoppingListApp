using Microsoft.AspNetCore.Mvc;

namespace AlisverisListesi.Controllers
{
    public class AdminPageController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
