using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Helpers;

namespace fut7Manager.Api.Services.Interfaces {
    public interface IPaymentService {
        Task<PagedResult<PaymentDto>> GetPaymentsAsync(int? teamId, int? leagueId, PaginationParams pagination);

        Task<PaymentDto?> GetPaymentByIdAsync(int id);

        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto);

        //Task<bool> UpdatePaymentAsync(int id, UpdatePaymentDto dto);

        Task<bool> DeletePaymentAsync(int id);
    }
}
