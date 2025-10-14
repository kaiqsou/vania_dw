using ProjetoMongoDB.Models;

namespace ProjetoMongoDB.Services
{
    public static class EventoNotifier
    {
        public delegate Task RegistroHandler(RegistroEventArgs args);
        public static event RegistroHandler? OnParticipanteRegistrado;
        public static async Task DispararRegistro(ApplicationUser participante, Evento evento)
        {
            if (OnParticipanteRegistrado != null)
            {
                // 
                await OnParticipanteRegistrado.Invoke(new RegistroEventArgs(participante, evento));
            }
        }
    }
}
