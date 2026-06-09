using AutoMapper;
using FifaPollApi.Domain.Entities;
using FifaPollApi.DTOs.Auth;
using FifaPollApi.DTOs.Team;
using FifaPollApi.DTOs.Vote;
using FifaPollApi.DTOs.Setting;

namespace FifaPollApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Team maps
            CreateMap<Team, TeamDto>().ReverseMap();
            CreateMap<CreateTeamDto, Team>();

            // Vote maps
            CreateMap<Vote, VoteDto>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.TeamName : string.Empty))
                .ForMember(dest => dest.FlagUrl, opt => opt.MapFrom(src => src.Team != null ? src.Team.FlagUrl : string.Empty))
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.Team != null ? src.Team.CountryCode : string.Empty));

            // Setting maps
            CreateMap<Setting, SettingDto>().ReverseMap();
        }
    }
}
