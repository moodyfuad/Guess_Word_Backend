using Guess_Word_Backend.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordleServer.Models
{
    public class Game
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string GameKey { get; set; } = string.Empty; // short public key clients share

        [Required]
        public int WordLength { get; set; } = 5;

        [Required]
        public int MaxAttempts { get; set; } = 6;

        [Required]
        public string Phase { get; set; } = GamePhase.WaitingForPlayers;

        // Index of the player whose turn it is (0 or 1). Null until game begins.
        public int? CurrentTurn { get; set; }

        // Players: exactly two players will be added
        public List<Player> Players { get; set; } = new();

        public List<Guess> Guesses { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Timestamp]
        public byte[]? RowVersion { get; set; } // concurrency
    }
}
