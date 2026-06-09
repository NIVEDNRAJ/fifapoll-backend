using System;

namespace FifaPollApi.Domain.Entities
{
    public class Vote
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Team? Team { get; set; }
    }
}
