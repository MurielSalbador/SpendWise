using Microsoft.AspNetCore.Mvc;
using SpendWise.Core.Services;

namespace SpendWise.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ExchangeRateService _exchangeRateService;

        public CurrencyController(ExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet("convert")]
        public async Task<IActionResult> Convert([FromQuery] decimal amount = 1)
        {
            var (rate, converted) = await _exchangeRateService.ConvertUsdToArsAsync(amount);

            if (rate == null || converted == null)
                return BadRequest("No se pudo obtener el tipo de cambio.");

            return Ok(new
            {
                from = "USD",
                to = "ARS",
                amount,
                rate,
                converted
            });
        }
    }
}

