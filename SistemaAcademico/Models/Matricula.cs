using System.ComponentModel.DataAnnotations;

namespace SistemaAcademico.Models
{
    public class Matricula
    {
        public int MatriculaId { get; set; }
        public DateTime? Data { get; set; }
        public Curso? Curso { get; set; } // relação da classe por objeto de Curso
        [Display(Name = "Curso")]
        public int IdCurso { get; set; } // chave estrangeira do Curso
        public Aluno? Aluno { get; set; } // relação da classe por objeto de Aluno
        [Display(Name = "Aluno")]
        public int IdAluno { get; set; } // chave estrangeira do Aluno
    }
}
