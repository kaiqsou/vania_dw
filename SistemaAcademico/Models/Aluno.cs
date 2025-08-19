using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAcademico.Models
{
    public class Aluno
    {
        public int AlunoId { get; set; }
        public string? Ra { get; set; }
        public Usuario? Usuario { get; set; } // relação da classe por objeto
        public int UsuarioId { get; set; } // chave estrangeira do Usuario
    }
}
