using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ProjetoMongoDB.Models;
using ProjetoMongoDB.Services;
using ProjetoMongoDB.ViewModels;
using Rotativa.AspNetCore;

namespace ProjetoMongoDB.Controllers
{
    public class EventoController : Controller
    {
        private readonly ContextMongoDb _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventoController(ContextMongoDb context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Evento
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Evento.Find(_ => true).ToListAsync());
        }

        // GET: Evento/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Evento.Find(m => m.Id == id).FirstOrDefaultAsync();

            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // GET: Evento/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Evento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Data,Tipo,HorarioInicio,HorarioFim")] Evento evento)
        {
            if (ModelState.IsValid)
            {
                evento.Id = Guid.NewGuid();
                await _context.Evento.InsertOneAsync(evento);
                return RedirectToAction(nameof(Index));
            }
            return View(evento);
        }

        // GET: Evento/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Evento.Find(m => m.Id == id).FirstOrDefaultAsync();

            if (evento == null)
            {
                return NotFound();
            }
            return View(evento);
        }

        // POST: Evento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome,Descricao,Data,Tipo,HorarioInicio,HorarioFim")] Evento eventoAtualizado)
        {
            if (id != eventoAtualizado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. CARREGAR O EVENTO ORIGINAL
                    var eventoOriginal = await _context.Evento
                        .Find(m => m.Id == eventoAtualizado.Id)
                        .FirstOrDefaultAsync();

                    if (eventoOriginal == null)
                    {
                        return NotFound();
                    }

                    // 2. ATUALIZAR OS DADOS DO EVENTO ORIGINAL (PRESERVANDO PARTICIPANTES)
                    eventoOriginal.Nome = eventoAtualizado.Nome;
                    eventoOriginal.Descricao = eventoAtualizado.Descricao;
                    eventoOriginal.Data = eventoAtualizado.Data; // GARANTINDO QUE A DATA ESTÁ AQUI
                    eventoOriginal.HorarioInicio = eventoAtualizado.HorarioInicio;
                    eventoOriginal.HorarioFim = eventoAtualizado.HorarioFim;
                    eventoOriginal.Tipo = eventoAtualizado.Tipo;

                    // 3. SUBSTITUIR O DOCUMENTO COMPLETO
                    await _context.Evento.ReplaceOneAsync(m => m.Id == eventoOriginal.Id, eventoOriginal);
                }
                catch (MongoException)
                {
                    if (!EventoExists(eventoAtualizado.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(eventoAtualizado);
        }

        // GET: Evento/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Evento.Find(m => m.Id == id).FirstOrDefaultAsync();

            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // POST: Evento/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _context.Evento.DeleteOneAsync(u => u.Id == id);

            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(Guid id) // por ser privado, apenas a própria classe pode executar ele
        {
            return _context.Evento.Find(e => e.Id == id).Any();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Participante")]
        public async Task<IActionResult> Registrar(Guid id)
        {
            // obter o usuário logado
            var user = await _userManager.GetUserAsync(User);

            // buscar o evento pelo id
            var evento = await _context.Evento.Find(e => e.Id == id).FirstOrDefaultAsync();

            if (evento == null)
            {
                return NotFound();
            }

            var filter = Builders<Evento>.Filter.Eq(e => e.Id, id);
            var update = Builders<Evento>.Update.AddToSet(e => e.Participantes, user.Id);
            var result = await _context.Evento.UpdateOneAsync(filter, update);

            if (result.IsAcknowledged)
            {
                if (result.ModifiedCount > 0)
                {
                    // disparar um evento que envia o e-mail para o inscrito
                    await EventoNotifier.DispararRegistro(user, evento);

                    TempData["Message"] = "Inscrição realizada com sucesso. Você receberá um e-mail com mais informações do evento";
                }
                else
                {
                    TempData["Message"] = "Você já está inscrito!";
                }
            }
            else
            {
                TempData["Error"] = "Tente novamente mais tarde! Sua inscrição não foi realizada";
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Participante")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(Guid id)
        {
            // Busca o usuário logado
            var user = await _userManager.GetUserAsync(User);

            // Busca o Evento com o Id recebido para cancelamento
            var evento = await _context.Evento.Find(e => e.Id == id).FirstOrDefaultAsync();

            // Verifica se user ou evento retornou nulo, e caso sim, dará NotFound
            if (user == null || evento == null)
            {
                return NotFound();
            }

            // Verifica se o Id do usuário está na lista de Participantes do evento
            if (evento.Participantes.Contains(user.Id))
            {
                // Filtro MongoDB para encontrar o evento com o Id correspondente - Equal
                var filter = Builders<Evento>.Filter.Eq(e => e.Id, id);

                // Operação de atualização que remove o Id do usuário da lista de Participantes - Pull
                var update = Builders<Evento>.Update.Pull(e => e.Participantes, user.Id);

                // Executa a atualização, aplicando o Filter e o Update criados
                var result = await _context.Evento.UpdateOneAsync(filter, update);

                // Verifica se a atualização foi bem-sucedida e se alterou linhas, exibindo mensagens diferentes para caso sim ou caso não
                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    TempData["Message"] = "Sua inscrição foi cancelada com sucesso!";
                }
                else
                {
                    TempData["Error"] = "Não foi possível cancelar a sua inscrição.";
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Participante")]
        public async Task<IActionResult> MeusCertificados()
        {
            var user = await _userManager.GetUserAsync(User);
            var agora = DateTime.Now;
            var dataAtual = DateOnly.FromDateTime(agora);

            var eventosPassados = await _context.Evento.Find(e => e.Participantes.Contains(user.Id) && (e.Data < dataAtual)).ToListAsync();

            return View(eventosPassados);
        }

        [Authorize(Roles = "Participante")]
        public async Task<IActionResult> Emitir(Guid id)
        {
            // Buscar os dados do usuário logado
            var user = await _userManager.GetUserAsync(User);

            // Buscar os dados do evento
            var evento = await _context.Evento.Find(e => e.Id == id).FirstOrDefaultAsync();

            if (evento != null)
            {
                return NotFound();
            }

            // Criar os dados do certificado
            var cargaHoraria = evento.HorarioFim - evento.HorarioInicio;
            // TimeSpan cargaHoraria = evento.HorarioFim.Subtract(evento.HorarioInicio);

            var model = new CertificadoViewModel()
            {
                Titulo = "Certificado",
                NomeParticipante = user.NomeCompleto,
                NomeEvento = evento.Nome,
                Data = evento.Data,
                TipoEvento = evento.Tipo,
                CargaHoraria = Convert.ToInt32(cargaHoraria)
            };

            // Passar para a View Certificado
            return new ViewAsPdf("Certificado", model)
            {
                FileName = "Certificado.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageMargins = new Rotativa.AspNetCore.Options.Margins { Top = 10, Bottom = 10 }
            };
        }
    } //fim da classe
}
