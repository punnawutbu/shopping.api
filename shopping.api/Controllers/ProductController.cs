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
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductFacades _facade;
        public ProductController(ILogger<ProductController> logger, IProductFacades facade)
        {
            _logger = logger;
            _facade = facade;
        }
        [HttpGet()]
        public async Task<ActionResult<ProductData>> GetProduct([FromHeader(Name = "x-system-key")] string SystemId)
        {
            _logger.LogInformation("System: {@SystemId}", new { SystemId });
            var resp = await _facade.GetProductData();
            if (resp.Message == Message.Success)
            {
                return Ok(resp);
            }
            return BadRequest();
        }
    }
}