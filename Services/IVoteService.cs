using System.Collections.Generic;
using System.Threading.Tasks;
using FifaPollApi.Common;
using FifaPollApi.DTOs.Vote;

namespace FifaPollApi.Services
{
    public interface IVoteService
    {
        Task<ApiResponse<VoteDto>> CastVoteAsync(int userId, VoteRequestDto voteRequestDto);
        Task<ApiResponse<VoteDto>> GetActiveVoteAsync(int userId);
        Task<ApiResponse<bool>> RevokeVoteAsync(int userId);
        Task<ApiResponse<IEnumerable<VoteResultDto>>> GetVoteResultsAsync(bool isAdmin = false);
        Task<ApiResponse<IEnumerable<VoterDetailsDto>>> GetVoterDetailsAsync();
    }
}
