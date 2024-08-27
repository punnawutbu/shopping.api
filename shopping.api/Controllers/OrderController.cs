using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using shopping.api.Shared.Constant;
using shopping.api.Shared.Facades;
using shopping.api.Shared.Models;

namespace shopping.api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderFacades _facade;
        public OrderController(ILogger<OrderController> logger, IOrderFacades facade)
        {
            _logger = logger;
            _facade = facade;
        }
        [HttpGet("ShoppingCart")]
        public async Task<ActionResult<ProductData>> GetShoppingCart([FromHeader(Name = "x-system-key")] string SystemId, [FromQuery] string orderNo)
        {
            _logger.LogInformation("System: {@SystemId}", new { SystemId });
            var resp = await _facade.GetShoppingCartDetail(orderNo);
            if (resp.Message == Message.Success)
            {
                return Ok(resp);
            }
            return BadRequest();
        }
        [HttpPost()]
        public async Task<ActionResult<ShopingCart>> OrderShoppingCart([FromHeader(Name = "x-system-key")] string SystemId, ShopingCart cart)
        {
            _logger.LogInformation("System: {@SystemId}", new { SystemId });
            var resp = await _facade.OrderShoppingCart(cart);
            if (resp.Message == Message.Success)
            {
                return Ok(resp);
            }
            return BadRequest();
        }
        [HttpDelete("delete")]
        public async Task<ActionResult<ShopingCart>> DeleteShoppingCart([FromHeader(Name = "x-system-key")] string SystemId, [FromQuery] string orderNo)
        {
            _logger.LogInformation("System: {@SystemId}", new { SystemId });
            var resp = await _facade.DeleteShoppingCart(orderNo);
            if (resp.Message == Message.Success)
            {
                return Ok(resp);
            }
            return BadRequest();
        }
    }
}