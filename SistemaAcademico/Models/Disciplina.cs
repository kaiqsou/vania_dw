namespace SistemaAcademico.Models
{
    public class Disciplina
    {
        public int DisciplinaId { get; set; }
        public string? Nome { get; set; }
        public Curso? Curso { get; set; } // relação da classe por objeto de Curso
        public int CursoId {  get; set; } // chave estrangeira do Curso
    }
}
