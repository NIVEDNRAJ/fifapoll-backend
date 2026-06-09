using System;

namespace FifaPollApi.Domain.Entities
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty; // e.g. ARG, BRA
        public string FlagUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
