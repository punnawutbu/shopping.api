using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using shopping.api.Shared.Models;

namespace shopping.api.Shared.Repositories
{
    public class ShoppingRepository : BaseRepository, IShoppingRepository
    {
        public ShoppingRepository(string connectionString, bool sslMode, Certificate certs) : base(connectionString, sslMode, certs) { }
        public async Task<IEnumerable<ProductData>> GetProductDataAsync()
        {
            using (var sqlConnection = OpenDbConnection())
            {
                var sql = @"
                    select	p.product_code as ProductCode,
                            p.product_name as ProductName,
                            p.price,
                            s.quantity
                    from public.product p left join public.stock s
                        on p.product_code = s.product_code;
                ";
                return await sqlConnection.QueryAsync<ProductData>(sql);
            }
        }
        public async Task<int> GetQuanityStock(string productCode)
        {
            using (var sqlConnection = OpenDbConnection())
            {
                var sql = @"
                    select	quantity
                    from public.stock
                    where product_code = @ProductCode;
                ";
                return await sqlConnection.QueryFirstOrDefaultAsync<int>(sql, new { ProductCode = productCode });
            }
        }
        public async Task<int> InsertOrder(string orderNo)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var sqlConnection = OpenDbConnection())
                {
                    var sql = @"
                    INSERT INTO public.order(
	                    order_no, order_status)
	                    VALUES (@OrderNo, 'Active') returning id;";
                    int id = await sqlConnection.ExecuteScalarAsync<int>(sql, new { OrderNo = orderNo });

                    if (id == 0)
                    {
                        transactionScope.Dispose();
                    }
                    transactionScope.Complete();
                    return id;
                }
            }
        }
        public async Task<bool> InsertOrderDetail(ShopingCart cart)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var sqlConnection = OpenDbConnection())
                {
                    var sql = @"
                    INSERT INTO public.order_detail(
                        product_code, product_name, price, order_id, quantity)
                        VALUES (@ProductCode, @ProductName, @Price, @OrderId, @Quantity);";
                    int rowsEffected = await sqlConnection.ExecuteAsync(sql, cart);

                    if (rowsEffected == 0)
                    {
                        transactionScope.Dispose();
                        return false;
                    }
                    transactionScope.Complete();
                    return true;
                }
            }
        }
        public async Task<int> GetOrderIdByOrderNo(string orderNo)
        {
            using (var sqlConnection = OpenDbConnection())
            {
                var sql = @"
                    select	id
                    from public.order
                    where order_no = @OrderNo
                    and order_status = 'Active';
                ";
                return await sqlConnection.QueryFirstOrDefaultAsync<int>(sql, new { OrderNo = orderNo });
            }
        }
        public async Task<IEnumerable<ShopingCart>> GetOrderDetail(int orderId)
        {
            using (var sqlConnection = OpenDbConnection())
            {
                var sql = @"
                    SELECT  product_code as ProductCode,
                            product_name as ProductName,
                            price,
                            order_id as OrderId,
                            quantity
	                FROM public.order_detail
                    where order_id = @OrderId
                    and quantity > 0;
                ";
                return await sqlConnection.QueryAsync<ShopingCart>(sql, new { OrderId = orderId });
            }
        }
        public async Task<int> UpdateStock(string productCode, int quantity)
        {
            using (var sqlConnection = OpenDbConnection())
            {
                quantity = -quantity;
                var sql = @"
                    UPDATE public.stock
                    SET quantity = quantity + @Quantity
                    WHERE product_code = @ProductCode;
                ";
                return await sqlConnection.ExecuteAsync(sql, new { ProductCode = productCode, Quantity = quantity });
            }
        }
        public async Task<bool> CheckItemInCart(int orderId, string productCode)
        {
            using (var sqlConnection = OpenDbConnection())
            {
                var sql = @"
                    select	id
                    from public.order_detail
                    where order_id = @OrderId
                    and product_code = @ProductCode;
                ";
                var id = await sqlConnection.QueryFirstOrDefaultAsync<int>(sql, new { OrderId = orderId, ProductCode = productCode });
                return id > 0;
            }
        }
        public async Task<int> UpdateQuantityOrder(int orderId, string productCode, int quantity)
        {
            using (var sqlConnection = OpenDbConnection())
            {
                quantity = -quantity;
                var sql = @"
                    UPDATE public.order_detail
                    SET quantity = quantity + @Quantity
                    where order_id = @OrderId
                    and product_code = @ProductCode;
                ";
                return await sqlConnection.ExecuteAsync(sql, new { OrderId = orderId, ProductCode = productCode, Quantity = quantity });
            }
        }
        public async Task<bool> DeleteOrder(int orderId)
        {
            using (var sqlConnection = OpenDbConnection())
            {
                var sqlOrder = @"DELETE FROM public.order WHERE id = @OrderId;";
                var sqlOrderD    = @"DELETE FROM public.order_detail WHERE order_id = @OrderId;";
                int orderEffected = await sqlConnection.ExecuteAsync(sqlOrder, new { OrderId = orderId });
                int orderDetailEffected = await sqlConnection.ExecuteAsync(sqlOrderD, new { OrderId = orderId });

                return orderEffected > 0 && orderDetailEffected > 0;
            }
        }
    }
}