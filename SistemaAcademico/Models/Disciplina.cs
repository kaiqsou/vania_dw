using System.ComponentModel.DataAnnotations;

namespace SistemaAcademico.Models
{
    public class Disciplina
    {
        public int DisciplinaId { get; set; }
        [Required (ErrorMessage = "O nome da disciplina é obrigatório")]
        [Display(Name = "Nome do Curso")]
        public string? Nome { get; set; }
        public Curso? Curso { get; set; } // relação da classe por objeto de Curso
        [Display(Name = "Cursos")]
        public int CursoId {  get; set; } // chave estrangeira do Curso
    }
}
