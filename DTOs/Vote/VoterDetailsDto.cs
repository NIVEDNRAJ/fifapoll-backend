using System;

namespace FifaPollApi.DTOs.Vote
{
    public class VoterDetailsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public bool HasVoted { get; set; }
        public int? VotedTeamId { get; set; }
        public string VotedTeamName { get; set; } = string.Empty;
        public DateTime? VotedAt { get; set; }
    }
}
