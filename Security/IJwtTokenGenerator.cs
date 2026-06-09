using FifaPollApi.Domain.Entities;

namespace FifaPollApi.Security
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
