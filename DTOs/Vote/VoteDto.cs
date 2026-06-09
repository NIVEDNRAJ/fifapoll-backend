using System;

namespace FifaPollApi.DTOs.Vote
{
    public class VoteDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string FlagUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime VotedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
