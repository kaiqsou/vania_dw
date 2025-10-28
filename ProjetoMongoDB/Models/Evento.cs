using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjetoMongoDB.Models
{
    public class Evento
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [Required, Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required, Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required, Display(Name = "Data")]
        public DateOnly Data { get; set; }

        [Required, Display(Name = "Tipo de Evento")]
        public string Tipo { get; set; }

        [Required, Display(Name = "Horário de Início")]
        public TimeSpan HorarioInicio { get; set; }

        [Required, Display(Name = "Horário de Fim")]
        public TimeSpan HorarioFim { get; set; }

        [Display(Name = "Participantes")]
        public ICollection<Guid> Participantes { get; set; } = new List<Guid>();
    }
}
