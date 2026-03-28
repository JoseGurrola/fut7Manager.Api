using AutoMapper;
using AutoMapper.QueryableExtensions;
using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Extensions;
using fut7Manager.Api.Helpers;
using fut7Manager.Api.Models;
using fut7Manager.Api.Services.Interfaces;
using fut7Manager.Data;
using Microsoft.EntityFrameworkCore;

namespace fut7Manager.Api.Services {
    public class PaymentService : IPaymentService {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PaymentService(ApplicationDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<PaymentDto>> GetPaymentsAsync(int? teamId, int? leagueId, PaginationParams pagination) {
            var query = _context.Payments
                .AsNoTracking()
                .AsQueryable();

            // teamId
            if (teamId.HasValue)
                query = query.Where(p => p.TeamId == teamId.Value);

            if (leagueId.HasValue)
                query = query.Where(p => p.Team.LeagueId == leagueId.Value);

            // Proyección a DTO después de aplicar filtros
            var dtoQuery = query
                .ProjectTo<PaymentDto>(_mapper.ConfigurationProvider);

            return await dtoQuery.ToPagedResultAsync(q => q.OrderBy(x => x.Id),
                pagination.PageNumber,
                pagination.PageSize);
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(int id) {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.Id == id)
                .ProjectTo<PaymentDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto) {
            var teamExists = await _context.Teams
                .AnyAsync(t => t.Id == dto.TeamId);

            if (!teamExists)
                throw new KeyNotFoundException("Team not found");

            var payment = _mapper.Map<Payment>(dto);
            payment.Date = DateTime.UtcNow;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }

        //public async Task<bool> UpdatePaymentAsync(int id, UpdatePaymentDto dto) {
        //    var payment = await _context.Payments.FindAsync(id);

        //    if (payment == null)
        //        return false;

        //    _mapper.Map(dto, payment);

        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public async Task<bool> DeletePaymentAsync(int id) {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
                return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
