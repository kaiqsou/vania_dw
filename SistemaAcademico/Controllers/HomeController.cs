using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SistemaAcademico.Models;

namespace SistemaAcademico.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(); // executa a view de mesmo nome do m�todo (Index)
        }

        public IActionResult Privacy()
        {
            return View(); // executa a view de mesmo nome do m�todo (Privacy)
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
