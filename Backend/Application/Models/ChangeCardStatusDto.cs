using Final_year_Project.Domain.EnumTypes;
using System.Text.Json.Serialization;

namespace Final_year_Project.Application.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public class ChangeCardStatusDto
    {
        public CardStatus Status { get; set; }
    }

}
