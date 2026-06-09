namespace FifaPollApi.Domain.Entities
{
    public class Setting
    {
        public int Id { get; set; }
        public bool IsVotingEnabled { get; set; } = true;
        public bool IsResultPublished { get; set; } = false;
    }
}
