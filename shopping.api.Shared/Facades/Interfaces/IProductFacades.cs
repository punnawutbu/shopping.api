using System.Threading.Tasks;
using shopping.api.Shared.Models;
using shopping.api.Shared.ResponseMessage;

namespace shopping.api.Shared.Facades
{
    public interface IProductFacades
    {
        Task<ResponseMessage<ProductData>> GetProductData();
    }
}