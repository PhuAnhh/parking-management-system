namespace Final_year_Project.Application.Models
{
    public class ExitLogDto
    {
        public int Id { get; set; }

        public int EntryLogId { get; set; }

        public string? ExitPlateNumber { get; set; }

        public int CardId { get; set; }

        public int CardGroupId { get; set; }

        public int EntryLaneId { get; set; }

        public int ExitLaneId { get; set; }

        public DateTime EntryTime { get; set; }

        public DateTime ExitTime { get; set; }

        public long TotalDuration { get; set; }

        public decimal TotalPrice { get; set; }

        public string? Note { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public EntryLogDto? EntryLog { get; set; }

    }

    public class CreateExitLogDto
    {
        public int EntryLogId { get; set; }
        public string? ExitPlateNumber { get; set; }  
        public int ExitLaneId { get; set; }
        public DateTime ExitTime { get; set; } 
        public string? Note { get; set; }
        public string? ImageUrl { get; set; }
    }
}
