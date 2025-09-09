using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace ProjetoMongoDB.Models
{
    [CollectionName("Role")]
    public class ApplicationRole : MongoIdentityRole {}
}
