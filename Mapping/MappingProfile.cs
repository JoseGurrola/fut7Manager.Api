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
            CreateMap<Team, TeamDto>()
                .ForMember(d => d.Paid,
                    opt => opt.MapFrom(s => s.Payments.Sum(p => (decimal?)p.Amount) ?? 0))
                .ForMember(d => d.Remaining,
                    opt => opt.MapFrom(s =>
                        (s.League != null ? s.League.RegistrationFee : 0) -
                        (s.Payments.Sum(p => (decimal?)p.Amount) ?? 0)));

            CreateMap<CreateTeamDto, Team>()
                .ForMember(d => d.League, o => o.Ignore())
                .ForMember(d => d.Group, o => o.Ignore())
                .ForMember(d => d.Players, o => o.Ignore())
                .ForMember(d => d.Payments, o => o.Ignore())
                .ForMember(d => d.PositionTable, o => o.MapFrom(_ => 0))
                .ForMember(d => d.Points, o => o.MapFrom(_ => 0))
                .ForMember(d => d.GoalsFor, o => o.MapFrom(_ => 0))
                .ForMember(d => d.GoalsAgainst, o => o.MapFrom(_ => 0))
                .ForMember(d => d.TeamManagerName, o => o.MapFrom(s => s.TeamManagerName))
                .ForMember(d => d.TeamManagerPhone, o => o.MapFrom(s => s.TeamManagerPhone));

            CreateMap<UpdateTeamDto, Team>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.LeagueId, o => o.Ignore())
                //.ForMember(d => d.GroupId, o => o.Ignore())  ❌ quitar

                .ForMember(d => d.League, o => o.Ignore())
                .ForMember(d => d.Group, o => o.Ignore())
                .ForMember(d => d.Players, o => o.Ignore())
                .ForMember(d => d.Payments, o => o.Ignore())

                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.LogoUrl, o => o.MapFrom(s => s.LogoUrl))
                .ForMember(d => d.PositionTable, o => o.MapFrom(s => s.PositionTable))
                .ForMember(d => d.Points, o => o.MapFrom(s => s.Points))
                .ForMember(d => d.GoalsFor, o => o.MapFrom(s => s.GoalsFor))
                .ForMember(d => d.GoalsAgainst, o => o.MapFrom(s => s.GoalsAgainst))
                .ForMember(d => d.GroupId, o => o.MapFrom(s => s.GroupId))
                .ForMember(d => d.TeamManagerName, o => o.MapFrom(s => s.TeamManagerName))
                .ForMember(d => d.TeamManagerPhone, o => o.MapFrom(s => s.TeamManagerPhone));

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
            CreateMap<Matchday, MatchdayDto>()
                .ForMember(dest => dest.RestingTeamNames,
                    opt => opt.MapFrom(src => src.RestingTeamNames));

            //Payment
            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Date, opt => opt.Ignore());
            CreateMap<Payment, PaymentDto>()
                .ForMember(d => d.TeamName,
                    opt => opt.MapFrom(s => s.Team.Name));
        }
    }
}
