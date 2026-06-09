using System.Collections.Generic;
using System.Threading.Tasks;
using FifaPollApi.Common;
using FifaPollApi.DTOs.Team;

namespace FifaPollApi.Services
{
    public interface ITeamService
    {
        Task<ApiResponse<TeamDto>> GetTeamByIdAsync(int id);
        Task<ApiResponse<IEnumerable<TeamDto>>> GetAllTeamsAsync();
        Task<ApiResponse<TeamDto>> CreateTeamAsync(CreateTeamDto createTeamDto);
        Task<ApiResponse<TeamDto>> UpdateTeamAsync(int id, CreateTeamDto createTeamDto);
        Task<ApiResponse<bool>> DeleteTeamAsync(int id);
    }
}
