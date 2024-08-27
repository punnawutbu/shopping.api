
using System.Collections.Generic;

namespace shopping.api.Shared.Models
{
    public class ProductData
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    public class ShopingCart : ProductData
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
    }

    public class Order
    {
        public string OrderNo { get; set; }
        public List<ShopingCart> OrderDetail { get; set; }
        public decimal TotalPrice { get; set; }
    }
}