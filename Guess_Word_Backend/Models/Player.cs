using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordleServer.Models
{
    public class Player
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid GameId { get; set; }

        public Game? Game { get; set; }

        // Client-provided id (e.g., randomly generated GUID on the client) to identify socket mapping
        [Required]
        public string ClientId { get; set; } = string.Empty;

        // Player display name (optional)
        public string? DisplayName { get; set; }

        // *** DEMO ONLY: store plaintext secret to allow easy evaluation. REMOVE for production. ***
        public string? PlainSecret { get; set; }

        // Also keep hashed+salt fields if you want (not used in this demo)
        public string? SecretHash { get; set; } = string.Empty;
        public string? SecretSalt { get; set; } = string.Empty;

        // Whether player submitted secret
        public bool HasSubmittedSecret { get; set; } = false;

        // SignalR connection id (not persisted; may be managed in memory by hub)
        [NotMapped]
        public string? ConnectionId { get; set; }
    }
}
