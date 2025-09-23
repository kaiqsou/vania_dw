using System.ComponentModel.DataAnnotations;

namespace ProjetoMongoDB.Models
{
    public class UserRole
    {
        [Required]
        public string? RoleName { get; set; }
    }
}
