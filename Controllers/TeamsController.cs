using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FifaPollApi.Common;
using FifaPollApi.DTOs.Team;
using FifaPollApi.Services;

namespace FifaPollApi.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IValidator<CreateTeamDto> _createTeamValidator;

        public TeamsController(ITeamService teamService, IValidator<CreateTeamDto> createTeamValidator)
        {
            _teamService = teamService;
            _createTeamValidator = createTeamValidator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TeamDto>>>> GetAll()
        {
            var response = await _teamService.GetAllTeamsAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TeamDto>>> GetById(int id)
        {
            var response = await _teamService.GetTeamByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<TeamDto>>> Create([FromBody] CreateTeamDto createTeamDto)
        {
            var validationResult = await _createTeamValidator.ValidateAsync(createTeamDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<TeamDto>.Fail("Validation failed", errors));
            }

            var response = await _teamService.CreateTeamAsync(createTeamDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<TeamDto>>> Update(int id, [FromBody] CreateTeamDto createTeamDto)
        {
            var validationResult = await _createTeamValidator.ValidateAsync(createTeamDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<TeamDto>.Fail("Validation failed", errors));
            }

            var response = await _teamService.UpdateTeamAsync(id, createTeamDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var response = await _teamService.DeleteTeamAsync(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
