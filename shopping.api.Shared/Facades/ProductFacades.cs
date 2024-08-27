using System.Threading.Tasks;
using shopping.api.Shared.Models;
using shopping.api.Shared.Repositories;
using shopping.api.Shared.ResponseMessage;

namespace shopping.api.Shared.Facades
{
    public class ProductFacades : IProductFacades
    {
        private readonly IShoppingRepository _shoppingRepository;
        public ProductFacades(IShoppingRepository shoppingRepository)
        {
            _shoppingRepository = shoppingRepository;
        }
        public async Task<ResponseMessage<ProductData>> GetProductData()
        {
            var resp = new ResponseMessage<ProductData>
            {
                Message = Constant.Message.Fail
            };

            var productData = await _shoppingRepository.GetProductDataAsync();
            if (productData == null)
            {
                resp.Message = Constant.Message.NoData;
                return resp;
            }

            resp.Results = productData;
            resp.Message = Constant.Message.Success;

            return resp;
        }
    }
}