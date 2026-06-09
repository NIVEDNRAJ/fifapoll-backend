using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FifaPollApi.Common;
using FifaPollApi.Domain.Entities;
using FifaPollApi.Domain.Repositories;
using FifaPollApi.DTOs.Vote;

namespace FifaPollApi.Services
{
    public class VoteService : IVoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VoteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<VoteDto>> CastVoteAsync(int userId, VoteRequestDto voteRequestDto)
        {
            var settings = await _unitOfWork.Settings.GetSettingsAsync();
            if (settings == null)
            {
                settings = new Setting { IsVotingEnabled = true, IsResultPublished = false };
                // We don't save settings immediately, it'll save on CompleteAsync if added.
            }

            if (!settings.IsVotingEnabled)
            {
                return ApiResponse<VoteDto>.Fail("Voting is currently disabled.");
            }

            if (settings.IsResultPublished)
            {
                return ApiResponse<VoteDto>.Fail("Voting is locked because results have been published.");
            }

            var team = await _unitOfWork.Teams.GetByIdAsync(voteRequestDto.TeamId);
            if (team == null)
            {
                return ApiResponse<VoteDto>.Fail("Selected team does not exist.");
            }

            var activeVote = await _unitOfWork.Votes.GetActiveVoteByUserIdAsync(userId);
            if (activeVote != null)
            {
                if (activeVote.TeamId == voteRequestDto.TeamId)
                {
                    var existingDto = _mapper.Map<VoteDto>(activeVote);
                    return ApiResponse<VoteDto>.Ok(existingDto, "You have already voted for this team.");
                }

                // Change vote
                activeVote.TeamId = voteRequestDto.TeamId;
                activeVote.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Votes.Update(activeVote);
                await _unitOfWork.CompleteAsync();

                // Reload to populate Team details
                var updatedVote = await _unitOfWork.Votes.GetByIdAsync(activeVote.Id);
                var dto = _mapper.Map<VoteDto>(updatedVote);
                return ApiResponse<VoteDto>.Ok(dto, "Your vote has been updated successfully!");
            }

            // Cast new vote
            var vote = new Vote
            {
                UserId = userId,
                TeamId = voteRequestDto.TeamId,
                IsActive = true,
                VotedAt = DateTime.UtcNow
            };

            await _unitOfWork.Votes.AddAsync(vote);
            await _unitOfWork.CompleteAsync();

            var newVote = await _unitOfWork.Votes.GetByIdAsync(vote.Id);
            var newDto = _mapper.Map<VoteDto>(newVote);
            return ApiResponse<VoteDto>.Ok(newDto, "Vote cast successfully!");
        }

        public async Task<ApiResponse<VoteDto>> GetActiveVoteAsync(int userId)
        {
            var vote = await _unitOfWork.Votes.GetActiveVoteByUserIdAsync(userId);
            if (vote == null)
            {
                return ApiResponse<VoteDto>.Ok(null!, "No active vote found.");
            }

            var dto = _mapper.Map<VoteDto>(vote);
            return ApiResponse<VoteDto>.Ok(dto);
        }

        public async Task<ApiResponse<bool>> RevokeVoteAsync(int userId)
        {
            var settings = await _unitOfWork.Settings.GetSettingsAsync();
            if (settings == null)
            {
                settings = new Setting { IsVotingEnabled = true, IsResultPublished = false };
            }

            if (!settings.IsVotingEnabled)
            {
                return ApiResponse<bool>.Fail("Voting is currently disabled.");
            }

            if (settings.IsResultPublished)
            {
                return ApiResponse<bool>.Fail("Voting modification is locked because results have been published.");
            }

            var activeVote = await _unitOfWork.Votes.GetActiveVoteByUserIdAsync(userId);
            if (activeVote == null)
            {
                return ApiResponse<bool>.Fail("No active vote found to revoke.");
            }

            // Deactivate (soft delete)
            activeVote.IsActive = false;
            activeVote.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Votes.Update(activeVote);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Ok(true, "Your vote has been revoked successfully!");
        }

        public async Task<ApiResponse<IEnumerable<VoteResultDto>>> GetVoteResultsAsync(bool isAdmin = false)
        {
            var settings = await _unitOfWork.Settings.GetSettingsAsync();
            if (settings == null)
            {
                settings = new Setting { IsVotingEnabled = true, IsResultPublished = false };
            }

            if (!settings.IsResultPublished && !isAdmin)
            {
                return ApiResponse<IEnumerable<VoteResultDto>>.Fail("Results are hidden until published by the admin.");
            }

            var teams = await _unitOfWork.Teams.GetAllAsync();
            var activeVotes = await _unitOfWork.Votes.GetAllActiveVotesAsync();

            var voteCounts = activeVotes
                .GroupBy(v => v.TeamId)
                .ToDictionary(g => g.Key, g => g.Count());

            int totalVotes = activeVotes.Count();

            var results = new List<VoteResultDto>();
            foreach (var team in teams)
            {
                int count = voteCounts.TryGetValue(team.Id, out var c) ? c : 0;
                double percentage = totalVotes > 0 ? Math.Round((double)count / totalVotes * 100, 2) : 0;

                results.Add(new VoteResultDto
                {
                    TeamId = team.Id,
                    TeamName = team.TeamName,
                    CountryCode = team.CountryCode,
                    FlagUrl = team.FlagUrl,
                    VoteCount = count,
                    Percentage = percentage
                });
            }

            // Sort results by count descending, then by name
            var sortedResults = results
                .OrderByDescending(r => r.VoteCount)
                .ThenBy(r => r.TeamName)
                .ToList();

            return ApiResponse<IEnumerable<VoteResultDto>>.Ok(sortedResults);
        }

        public async Task<ApiResponse<IEnumerable<VoterDetailsDto>>> GetVoterDetailsAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var activeVotes = await _unitOfWork.Votes.GetAllActiveVotesAsync();

            var activeVotesDict = activeVotes.ToDictionary(v => v.UserId, v => v);

            var voterDetailsList = new List<VoterDetailsDto>();
            foreach (var user in users)
            {
                // Exclude Admin from voter list if desired, but we can list all of them
                if (user.Role == "Admin") continue;

                var details = new VoterDetailsDto
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    UserEmail = user.Email,
                    HasVoted = activeVotesDict.TryGetValue(user.Id, out var vote),
                    VotedTeamId = vote?.TeamId,
                    VotedTeamName = vote?.Team?.TeamName ?? string.Empty,
                    VotedAt = vote?.VotedAt
                };

                voterDetailsList.Add(details);
            }

            return ApiResponse<IEnumerable<VoterDetailsDto>>.Ok(voterDetailsList);
        }
    }
}
