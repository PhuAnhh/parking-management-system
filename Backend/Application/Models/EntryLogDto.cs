namespace Final_year_Project.Application.Models
{
    public class EntryLogDto
    {
        public int Id { get; set; }

        public string? PlateNumber { get; set; }

        public int CardId { get; set; }

        public int? CardGroupId { get; set; }

        public int LaneId { get; set; }

        public int? CustomerId { get; set; }

        public DateTime EntryTime { get; set; }

        public string? ImageUrl { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool Exited { get; set; } = false;


    }

    public class CreateEntryLogDto
    {
        public string? PlateNumber { get; set; }

        public int CardId { get; set; }

        public int LaneId { get; set; }

        public DateTime EntryTime { get; set; }

        public string? ImageUrl { get; set; }

        public string? Note { get; set; }
    }
}
