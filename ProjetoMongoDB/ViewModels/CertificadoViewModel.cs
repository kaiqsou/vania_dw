namespace ProjetoMongoDB.ViewModels
{
    public class CertificadoViewModel
    {
        public string? Titulo { get; set; }
        public string? NomeEvento { get; set; }
        public string? NomeParticipante { get; set; }
        public DateOnly Data { get; set; }
        public int CargaHoraria { get; set; }
        public string? TipoEvento { get; set; }
    }
}
