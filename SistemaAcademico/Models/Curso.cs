namespace SistemaAcademico.Models
{
    public class Curso
    {
        public int CursoId { get; set; }
        public string? Nome { get; set; }
        public List<Disciplina>? Disciplinas { get; set; } // Lista de Disciplinas - pode ter mais de uma
    }
}
