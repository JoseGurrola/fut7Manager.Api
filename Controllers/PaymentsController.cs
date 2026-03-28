using fut7Manager.Api.DTOs.Requests;
using fut7Manager.Api.DTOs.Responses;
using fut7Manager.Api.Extensions;
using fut7Manager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fut7Manager.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService) {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments(
            [FromQuery] int? teamId,
            [FromQuery] int? leagueId,
            [FromQuery] PaginationParams pagination) {
            var result = await _paymentService.GetPaymentsAsync(teamId, leagueId, pagination);

            Response.AddPaginationHeader(result);

            return Ok(result.Items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPayment(int id) {
            var payment = await _paymentService.GetPaymentByIdAsync(id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpPost]
        public async Task<ActionResult<PaymentDto>> CreatePayment(CreatePaymentDto dto) {
            var payment = await _paymentService.CreatePaymentAsync(dto);

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id) {
            var result = await _paymentService.DeletePaymentAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
