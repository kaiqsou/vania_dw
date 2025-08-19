using System.ComponentModel.DataAnnotations;

namespace SistemaAcademico.Models
{
    public class Usuario
    {
        // [Key] -> caso não use o padrão do Framework pra definir a chave primária
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Por favor, preencha o nome.")] // mensagem de erro
        public string? Nome { get; set; }

        [Display(Name = "E-mail")] // nome customizado do campo
        [Required(ErrorMessage = "Por favor, preencha o e-mail.")] 
        public string? Email { get; set; }

        [Required(ErrorMessage = "Por favor, preencha a senha.")]
        public string? Senha { get; set; }
    }
}
