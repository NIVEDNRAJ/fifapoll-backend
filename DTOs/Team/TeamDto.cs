namespace FifaPollApi.DTOs.Team
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string FlagUrl { get; set; } = string.Empty;
    }
}
