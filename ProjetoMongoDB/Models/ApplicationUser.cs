using MongoDbGenericRepository.Attributes;

namespace ProjetoMongoDB.Models
{
    [CollectionName("Users")] // definição do nome da coleção no MongoDB
    public class ApplicationUser
    {
        public string NomeCompleto { get; set; }
    }
}
