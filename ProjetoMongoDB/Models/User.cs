using System.ComponentModel.DataAnnotations;

namespace ProjetoMongoDB.Models
{
    public class User
    {
        [Required, Display(Name = "Nome completo")]
        public string NomeCompleto { get; set; }

        [Required, Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required, Display(Name = "Senha")]
        public string Password { get; set; }
    }
}
