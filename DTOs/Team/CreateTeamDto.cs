namespace FifaPollApi.DTOs.Team
{
    public class CreateTeamDto
    {
        public string TeamName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string FlagUrl { get; set; } = string.Empty;
    }
}
