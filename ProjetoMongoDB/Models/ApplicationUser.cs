using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace ProjetoMongoDB.Models
{
    [CollectionName("Users")] // definição do nome da coleção no MongoDB
    public class ApplicationUser : MongoIdentityUser
    {
        // Não faz parte do Framework do Identity, então ele é acrescentado no banco
        public string NomeCompleto { get; set; }
    }
}
