using ProjetoMongoDB.Models;

namespace ProjetoMongoDB.Services
{
    public class RegistroEventArgs : EventArgs
    {
        public ApplicationUser Participante { get; }
        public Evento EventoRegistrado { get; }

        public RegistroEventArgs(ApplicationUser participante, Evento eventoRegistrado)
        {
            Participante = participante;
            EventoRegistrado = eventoRegistrado;
        }
    }
}
