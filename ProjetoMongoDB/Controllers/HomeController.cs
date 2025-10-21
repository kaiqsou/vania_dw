using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MongoDB.Driver;
using ProjetoMongoDB.Models;
using System.Diagnostics;

namespace ProjetoMongoDB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ContextMongoDb _context;
        private UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ContextMongoDb context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Evento.Find(_ => true).ToListAsync());
        }

        [Authorize(Roles = "Participante")]
        public async Task<IActionResult> MeusEventos()
        {
            var userName = User.Identity?.Name;

            ApplicationUser user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            var meusEventos = await _context.Evento.Find(e => e.Data > DateOnly.FromDateTime(DateTime.Now) && e.Participantes.Contains(user.Id)).ToListAsync();

            return View(meusEventos);
        }

        [Authorize(Roles = "Participante")]
        public async Task<IActionResult> Participacoes()
        {
            var userName = User.Identity?.Name;

            ApplicationUser user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            var participacoes = await _context.Evento.Find(e => e.Data <= DateOnly.FromDateTime(DateTime.Now) && e.Participantes.Contains(user.Id)).ToListAsync();

            return View(participacoes);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
