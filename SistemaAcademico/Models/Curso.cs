using System.ComponentModel.DataAnnotations;

namespace SistemaAcademico.Models
{
    public class Curso
    {
        public int CursoId { get; set; }
        [Required (ErrorMessage = "O nome do curso é obrigatório")]
        [Display (Name = "Nome do Curso")]
        public string? Nome { get; set; }
        public List<Disciplina>? Disciplinas { get; set; } // Lista de Disciplinas - pode ter mais de uma
    }
}
