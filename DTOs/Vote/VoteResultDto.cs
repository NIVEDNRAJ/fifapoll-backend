namespace FifaPollApi.DTOs.Vote
{
    public class VoteResultDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string FlagUrl { get; set; } = string.Empty;
        public int VoteCount { get; set; }
        public double Percentage { get; set; }
    }
}
