using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAcademico.Models
{
    public class Aluno
    {
        public int AlunoId { get; set; }
        [Required (ErrorMessage = "O RA é obrigatório"), Display(Name = "RA")]
        public string? Ra { get; set; }
        public Usuario? Usuario { get; set; } // relação da classe por objeto
        [Display(Name = "Usuários")]
        public int UsuarioId { get; set; } // chave estrangeira do Usuario
    }
}
