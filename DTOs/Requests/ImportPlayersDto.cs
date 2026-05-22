using fut7Manager.Api.DTOs.Responses;
using System.ComponentModel.DataAnnotations;

namespace fut7Manager.Api.DTOs.Requests {
    public class ImportPlayersDto {
        public List<CreatePlayerDto> Players { get; set; } = new();
    }
}
