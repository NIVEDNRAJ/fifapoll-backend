using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FifaPollApi.Common;
using FifaPollApi.Domain.Entities;
using FifaPollApi.Domain.Repositories;
using FifaPollApi.DTOs.Team;

namespace FifaPollApi.Services
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<TeamDto>> GetTeamByIdAsync(int id)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            if (team == null)
            {
                return ApiResponse<TeamDto>.Fail("Team not found.");
            }

            var dto = _mapper.Map<TeamDto>(team);
            return ApiResponse<TeamDto>.Ok(dto);
        }

        public async Task<ApiResponse<IEnumerable<TeamDto>>> GetAllTeamsAsync()
        {
            var teams = await _unitOfWork.Teams.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<TeamDto>>(teams);
            return ApiResponse<IEnumerable<TeamDto>>.Ok(dtos);
        }

        public async Task<ApiResponse<TeamDto>> CreateTeamAsync(CreateTeamDto createTeamDto)
        {
            if (await _unitOfWork.Teams.ExistsByNameAsync(createTeamDto.TeamName))
            {
                return ApiResponse<TeamDto>.Fail("A team with this name already exists.");
            }

            var team = _mapper.Map<Team>(createTeamDto);
            team.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Teams.AddAsync(team);
            await _unitOfWork.CompleteAsync();

            var dto = _mapper.Map<TeamDto>(team);
            return ApiResponse<TeamDto>.Ok(dto, "Team created successfully!");
        }

        public async Task<ApiResponse<TeamDto>> UpdateTeamAsync(int id, CreateTeamDto createTeamDto)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            if (team == null)
            {
                return ApiResponse<TeamDto>.Fail("Team not found.");
            }

            var existingTeam = await _unitOfWork.Teams.GetByNameAsync(createTeamDto.TeamName);
            if (existingTeam != null && existingTeam.Id != id)
            {
                return ApiResponse<TeamDto>.Fail("A team with this name already exists.");
            }

            _mapper.Map(createTeamDto, team);
            _unitOfWork.Teams.Update(team);
            await _unitOfWork.CompleteAsync();

            var dto = _mapper.Map<TeamDto>(team);
            return ApiResponse<TeamDto>.Ok(dto, "Team updated successfully!");
        }

        public async Task<ApiResponse<bool>> DeleteTeamAsync(int id)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            if (team == null)
            {
                return ApiResponse<bool>.Fail("Team not found.");
            }

            _unitOfWork.Teams.Delete(team);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Ok(true, "Team deleted successfully!");
        }
    }
}
