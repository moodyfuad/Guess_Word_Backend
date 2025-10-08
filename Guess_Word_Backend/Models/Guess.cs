using System.ComponentModel.DataAnnotations;

namespace WordleServer.Models
{
    public class Guess
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid GameId { get; set; }

        public Game? Game { get; set; }

        // Which player made the guess (0 or 1 index)
        public int PlayerIndex { get; set; }

        [Required]
        public string Word { get; set; } = string.Empty;

        // Per-letter feedback stored as CSV of ints (to keep simple), or store structured JSON in production
        public string? FeedbackCsv { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
