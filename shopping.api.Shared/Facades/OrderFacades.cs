
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopping.api.Shared.Models;
using shopping.api.Shared.Repositories;
using shopping.api.Shared.ResponseMessage;

namespace shopping.api.Shared.Facades
{
    public class OrderFacades : IOrderFacades
    {
        private readonly IShoppingRepository _shoppingRepository;
        public OrderFacades(IShoppingRepository shoppingRepository)
        {
            _shoppingRepository = shoppingRepository;
        }
        public async Task<ResponseMessage<Order>> GetShoppingCartDetail(string orderNo)
        {
            var resp = new ResponseMessage<Order>
            {
                Message = Constant.Message.NoData
            };

            var orderDetails = await _GetOrderDetail(orderNo);

            if (orderDetails != null)
            {
                resp.Result = orderDetails;
                resp.Message = Constant.Message.Success;
            }

            return resp;
        }
        public async Task<ResponseMessage<Order>> OrderShoppingCart(ShopingCart cart)
        {
            var resp = new ResponseMessage<Order>
            {
                Message = Constant.Message.Fail
            };

            int qtyBalance = await _shoppingRepository.GetQuanityStock(cart.ProductCode);

            if (qtyBalance < cart.Quantity || qtyBalance == 0)
            {
                resp.Message = Constant.Message.OutOfStock;
                return resp;
            }

            if (string.IsNullOrEmpty(cart.OrderNo))
            {
                cart.OrderNo = @"Order-" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
                cart.OrderId = await _shoppingRepository.InsertOrder(cart.OrderNo);
                await _shoppingRepository.InsertOrderDetail(cart);
                await _shoppingRepository.UpdateStock(cart.ProductCode, cart.Quantity);
            }
            else
            {
                cart.OrderId = await _shoppingRepository.GetOrderIdByOrderNo(cart.OrderNo);

                bool IsItemInCart = await _shoppingRepository.CheckItemInCart(cart.OrderId, cart.ProductCode);
                if (IsItemInCart)
                {
                    await _shoppingRepository.UpdateQuantityOrder(cart.OrderId, cart.ProductCode, cart.Quantity);
                    await _shoppingRepository.UpdateStock(cart.ProductCode, cart.Quantity);
                }
                else
                {
                    await _shoppingRepository.InsertOrderDetail(cart);
                    await _shoppingRepository.UpdateStock(cart.ProductCode, cart.Quantity);
                }
            }

            resp.Result = await _GetOrderDetail(cart.OrderNo);
            resp.Message = Constant.Message.Success;

            return resp;
        }
        public async Task<ResponseMessage<bool>> DeleteShoppingCart(string orderNo)
        {
            var resp = new ResponseMessage<bool>
            {
                Message = Constant.Message.Fail,
                Result = false,
            };
            var orderId = await _shoppingRepository.GetOrderIdByOrderNo(orderNo);
            await _shoppingRepository.DeleteOrder(orderId);
            resp.Result = true;
            resp.Message = Constant.Message.Success;

            return resp;
        }
        private async Task<Order> _GetOrderDetail(string orderNo)
        {

            var orderDetail = new Order
            {
                OrderNo = orderNo,
                OrderDetail = new List<ShopingCart>()
            };

            var orderId = await _shoppingRepository.GetOrderIdByOrderNo(orderNo);

            var getingOrderDetail = await _shoppingRepository.GetOrderDetail(orderId);

            orderDetail.OrderDetail.AddRange(getingOrderDetail.ToList());
            orderDetail.TotalPrice = orderDetail.OrderDetail.Sum(x => x.Price);

            return orderDetail;
        }
    }
}