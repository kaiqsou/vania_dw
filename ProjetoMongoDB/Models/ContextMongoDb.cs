using MongoDB.Driver;

namespace ProjetoMongoDB.Models
{
    // Início da Classe
    public class ContextMongoDb
    {
        public static string ConnectionString { get; set; }
        public static string DatabaseName { get; set; }
        public static bool Isssl { get; set; }
        public IMongoDatabase _database { get; }

        // Início do Construtor
        public ContextMongoDb()
        {
            try
            {
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));

                if (Isssl)
                {
                    settings.SslSettings = new SslSettings
                    {
                        EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                    };
                }

                var mongoclient = new MongoClient(settings);
                _database = mongoclient.GetDatabase(DatabaseName);
            }
            catch(Exception) 
            {
                throw new Exception("Não foi possível conectar no MongoDB");
            }
        } 
        // Fim do construtor
    }
    // Fim da Classe
}
