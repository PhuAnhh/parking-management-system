using System.Text.Json.Serialization;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Domain.EnumTypes;

namespace Final_year_Project.Application.Models
{
    public class CardDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int CardGroupId { get; set; }

        public int? CustomerId { get; set; }

        public string? Note { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardStatus Status { get; set; } 

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }

    public class CreateCardDto
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int CardGroupId { get; set; }

        public int? CustomerId { get; set; }

        public string? Note { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardStatus Status { get; set; }
    }

    public class UpdateCardDto
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int CardGroupId { get; set; }

        public int? CustomerId { get; set; }

        public string? Note { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardStatus Status { get; set; } 
    }
}
