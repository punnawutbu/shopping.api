using System.Threading.Tasks;
using shopping.api.Shared.Models;
using shopping.api.Shared.ResponseMessage;

namespace shopping.api.Shared.Facades
{
    public interface IOrderFacades
    {
        Task<ResponseMessage<Order>> OrderShoppingCart(ShopingCart cart);
        Task<ResponseMessage<bool>> DeleteShoppingCart(string orderNo);
        Task<ResponseMessage<Order>> GetShoppingCartDetail(string orderNo);
    }
}