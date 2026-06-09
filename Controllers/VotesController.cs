using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FifaPollApi.Common;
using FifaPollApi.DTOs.Vote;
using FifaPollApi.Services;

namespace FifaPollApi.Controllers
{
    [ApiController]
    [Route("api/votes")]
    [Authorize]
    public class VotesController : ControllerBase
    {
        private readonly IVoteService _voteService;

        public VotesController(IVoteService voteService)
        {
            _voteService = voteService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<VoteDto>>> CastVote([FromBody] VoteRequestDto voteRequestDto)
        {
            int userId = GetUserId();
            var response = await _voteService.CastVoteAsync(userId, voteRequestDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("my-vote")]
        public async Task<ActionResult<ApiResponse<VoteDto>>> GetMyVote()
        {
            int userId = GetUserId();
            var response = await _voteService.GetActiveVoteAsync(userId);
            return Ok(response);
        }

        [HttpPost("revoke")]
        public async Task<ActionResult<ApiResponse<bool>>> RevokeVote()
        {
            int userId = GetUserId();
            var response = await _voteService.RevokeVoteAsync(userId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("results")]
        [AllowAnonymous] // Anyone can view results if published
        public async Task<ActionResult<ApiResponse<IEnumerable<VoteResultDto>>>> GetResults()
        {
            bool isAdmin = User.Identity?.IsAuthenticated == true && User.IsInRole("Admin");
            var response = await _voteService.GetVoteResultsAsync(isAdmin);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("voters")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<VoterDetailsDto>>>> GetVoterDetails()
        {
            var response = await _voteService.GetVoterDetailsAsync();
            return Ok(response);
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            return int.Parse(userIdClaim.Value);
        }
    }
}
