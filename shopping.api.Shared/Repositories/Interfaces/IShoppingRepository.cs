using System.Collections.Generic;
using System.Threading.Tasks;
using shopping.api.Shared.Models;

namespace shopping.api.Shared.Repositories
{
    public interface IShoppingRepository
    {
        Task<IEnumerable<ProductData>> GetProductDataAsync();
        Task<int> GetQuanityStock(string productCode);
        Task<int> InsertOrder(string orderNo);
        Task<int> GetOrderIdByOrderNo(string orderNo);
        Task<IEnumerable<ShopingCart>> GetOrderDetail(int orderId);
        Task<bool> InsertOrderDetail(ShopingCart cart);
        Task<int> UpdateStock(string productCode, int quantity);
        Task<bool> CheckItemInCart(int orderId, string productCode);
        Task<int> UpdateQuantityOrder(int orderId, string productCode, int quantity);
        Task<bool> DeleteOrder(int orderId);
    }
}