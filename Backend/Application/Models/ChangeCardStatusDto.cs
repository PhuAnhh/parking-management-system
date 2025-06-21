using Final_year_Project.Domain.EnumTypes;
using System.Text.Json.Serialization;

namespace Final_year_Project.Application.Models
{
    public class ChangeCardStatusDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardStatus Status { get; set; }
    }

}
