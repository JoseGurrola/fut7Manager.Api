using AutoMapper;
using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Models;

namespace fut7Manager.Api.Mapping {
    public class MappingProfile : Profile {
        public MappingProfile() {
            // Player
            CreateMap<Player, PlayerDto>();
            CreateMap<CreatePlayerDto, Player>();
            CreateMap<UpdatePlayerDto, Player>();

            // Team
            CreateMap<Team, TeamDto>();
            CreateMap<CreateTeamDto, Team>();
            CreateMap<UpdateTeamDto, Team>();

            // League
            CreateMap<League, LeagueDto>();
            CreateMap<CreateLeagueDto, League>();

            // Match
            CreateMap<Fut7Match, Fut7MatchDto>()
                .ForMember(dest => dest.HomeTeamName,
                    opt => opt.MapFrom(src => src.HomeTeam.Name))
                .ForMember(dest => dest.AwayTeamName,
                    opt => opt.MapFrom(src => src.AwayTeam.Name));

            CreateMap<CreateFut7MatchDto, Fut7Match>();
            CreateMap<UpdateFut7MatchDto, Fut7Match>();

            //Group
            CreateMap<Group, GroupDto>();
            CreateMap<Group, GroupWithTeamsDto>();
            CreateMap<CreateGroupDto, Group>();

            CreateMap<Fut7Match, Fut7MatchDto>();
            CreateMap<Matchday, MatchdayDto>();
        }
    }
}
