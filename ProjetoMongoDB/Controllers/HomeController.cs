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
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var agora = DateTime.Now;
            var dataAtual = DateOnly.FromDateTime(agora);
            var horaAtual = agora.TimeOfDay;

            // Constru��o do Filtro com Builders<T>
            var filterBuilder = Builders<Evento>.Filter;

            // 1. Filtro de Inscri��o: e.Participantes.Contains(participanteId)
            // Usamos AnyEq, que � o m�todo recomendado do driver para o Contains em arrays/listas.
            var filtroInscricao = filterBuilder.AnyEq(e => e.Participantes, user.Id);

            // 2. Filtro de DURA��O (O evento ainda n�o acabou)
            // Condi��o 1: Eventos em datas estritamente futuras.
            // O evento de 29/10/2025 (ou posterior) � sempre v�lido.
            // e.Data > dataAtual
            var filtroDataFutura = filterBuilder.Gt(e => e.Data, dataAtual);

            // Condi��o 2: Eventos na data atual E com Hor�rioFim estritamente MAIOR que a hora atual.
            // Se o HorarioFim � 01:00:00 e a horaAtual � 01:03:46, este filtro ser� FALSO (01:00:00 > 01:03:46 � FALSO).
            // (e.Data == dataAtual && e.HorarioFim > horaAtual)
            var filtroMesmaDataComHoraValida = filterBuilder.And(filterBuilder.Eq(e => e.Data, dataAtual), filterBuilder.Gt(e => e.HorarioFim, horaAtual));

            var filtroDuracao = filterBuilder.Or(filtroDataFutura, filtroMesmaDataComHoraValida);
            var filtroCombinado = filterBuilder.And(filtroInscricao, filtroDuracao);

            var eventos = await _context.Evento.Find(filtroCombinado).ToListAsync();
         // var meusEventos = await _context.Evento.Find(e => e.Data > dataAtual && e.Participantes.Contains(user.Id)).ToListAsync();

            return View(eventos);
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
