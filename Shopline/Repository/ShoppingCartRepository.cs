using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Dapper;

public class ShoppingCartRepository
{
    private readonly DapperContext _context;

    public ShoppingCartRepository()
    {
        _context = new DapperContext();
    }

    public List<ShoppingCart> GetShoppingCart(string userName)
    {
        List<ShoppingCart> result = new List<ShoppingCart>();
        using (var connection = _context.GetConnection())
        {
            string sql = @"SELECT OrderDetail.ProductName,
                            OrderDetail.Price,
                            OrderDetail.Count,
                            OrderDetail.SellerAccount,
                            OrderDetail.ProductID,
                            OrderDetail.BuyerName,
                            OrderDetail.Status,
                            OrderDetail.ID,
                            Login.SellerName,
                            Product_Index.Image
                            FROM OrderDetail 
                            JOIN Product_Index ON OrderDetail.ProductID = Product_Index.Id
                            JOIN Login ON OrderDetail.SellerAccount = Login.UserName
                            WHERE OrderDetail.BuyerName = @UserName AND OrderDetail.Status = N'未結帳'";
            result = connection.Query<ShoppingCart>(sql, new { UserName = userName }).AsList();
            return result;
        }
    }

    public bool CreateShoppingCart(ShoppingCart create)
    {
        using (var connection = _context.GetConnection())
        {
            string sql = @"INSERT INTO OrderDetail (ProductName, Price, Count,SellerAccount,ProductID,BuyerName,Status)
                           VALUES  (@ProductName, @Price, @Count,@SellerAccount,@ProductID,@BuyerName,@Status)";
            var result = connection.Execute(sql, create) > 0 ? true : false;

            return result;
        }
    }

    public int DeleteShoppingCart(string delete)
    {
        using (var connection = _context.GetConnection())
        {
            string sql = @"DELETE FROM  OrderDetail
                           WHERE Id = @Id";
            var result = connection.Execute(sql, new { Id = delete });
            return result;
        }
    }

    /// <summary>
    /// 生成訂單
    /// </summary>
    /// <param name="insert"></param>
    /// <returns></returns>
    public int InsertOrder(List<ShoppingCart> insert)
    {
        Dictionary<string, List<ShoppingCart>> sellerNameList = new Dictionary<string, List<ShoppingCart>>();
        string sql = string.Empty;
        using (var connection = _context.GetConnection())
        {
            foreach (var item in insert)
            {
                //從購物車DB撈資料並改掉
                sql = @"SELECT * FROM OrderDetail 
                           WHERE ID=@ID";
                var result = connection.QueryFirstOrDefault<ShoppingCart>(sql, item);
                result.Count = item.Count;
                //訂單依賣家做分類
                if (sellerNameList.ContainsKey(result.SellerAccount))
                {
                    sellerNameList[result.SellerAccount].Add(result);
                }
                else
                {
                    sellerNameList.Add(result.SellerAccount, new List<ShoppingCart>() { result });
                }
            }
            foreach (var key in sellerNameList)
            {
                string orderID = Guid.NewGuid().ToString();
                long totalPrice = 0;
                string buyName = string.Empty;
                string sellerAccount = key.Key;
                foreach (var item in key.Value)
                {
                    item.OrderID = orderID;
                    item.Status = "待付款";
                    sql = @"UPDATE OrderDetail
                               SET OrderID = @OrderID,
                                   Status= @Status,
                                   Count = @Count
                             WHERE ID=@ID";
                    connection.Execute(sql, item);
                    totalPrice += item.Price * item.Count;
                    buyName = item.BuyerName;
                }
                //寫入訂單model (OrderList)
                OrderList newOrder = new OrderList();
                newOrder.OrderNumber = orderID;
                newOrder.OrderDate = DateTime.Now.ToString();
                newOrder.TotalPrice = totalPrice.ToString();
                newOrder.BuyerName = buyName;
                newOrder.SellerAccount = sellerAccount;
                //寫入DB
                sql = @"INSERT INTO OrderList (OrderNumber,OrderDate,TotalPrice,BuyerName,SellerAccount)
                                    VALUES  (@OrderNumber,@OrderDate,@TotalPrice,@BuyerName,@SellerAccount)";
                connection.Execute(sql, newOrder);
            }
        }
        return 1;
    }

    /// <summary>
    /// 拿訂單&訂單詳細
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public List<OrderResult> GetOrder(string userName)
    {
        List<OrderResult> results = new List<OrderResult>();
        string sql = string.Empty;
        using (var connection = _context.GetConnection())
        {
            sql = @"SELECT * FROM OrderList
                     WHERE SellerAccount = @UserName
                     ORDER BY OrderDate DESC";
            var orders = connection.Query<OrderList>(sql, new { UserName = userName }).AsList();
            foreach (var item in orders)
            {
                OrderResult result = new OrderResult();
                result.OrderModel = item;
                sql = @"SELECT * FROM OrderDetail
                         WHERE OrderID = @OrderID";
                result.OrderValues = connection.Query<ShoppingCart>(sql, new { OrderID = item.OrderNumber }).AsList();
                results.Add(result);
            }

            return results;
        }
    }

    public int CancelShoppingCart(string delete)
    {
        using (var connection = _context.GetConnection())
        {
            string sql = @"UPDATE OrderDetail
                        SET Status= @Status
                        WHERE ID = @ID ";
            var result = connection.Execute(sql, new { ID = delete, Status = "已取消" });
            return result;
        }
    }

    public bool UpdateShoppingCart(ShoppingCart edit)
    {
        string sql = string.Empty;
        using (var connection = _context.GetConnection())
        {
            sql = @"UPDATE OrderDetail 
                            SET Count = @Count
                            WHERE ID=@ID ";
            var result = connection.Execute(sql, edit) > 0 ? true : false;
            sql = @"SELECT * FROM OrderDetail
                    WHERE ID = @ID";
            var orderDetail = connection.QueryFirstOrDefault<ShoppingCart>(sql, edit);

            sql = @"SELECT * FROM OrderDetail
                    WHERE OrderID = @OrderID";
            var orderDetails = connection.Query<ShoppingCart>(sql, orderDetail).AsList();

            int totalPrice = 0;
            foreach (var item in orderDetails)
            {
                totalPrice += item.Price * item.Count;
            }
            sql = @"UPDATE OrderList
                       SET TotalPrice = @TotalPrice
                     WHERE OrderNumber = @OrderNumber";
            result = connection.Execute(sql, new { TotalPrice = totalPrice, OrderNumber = orderDetail.OrderID }) > 0 ? true : false;
            return result;
        }
    }

    /// <summary>
    /// 報表
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    public ReportData Report(ReportViewModel report)
    {
        int todayCount = 0;
        int thisMonthCount = 0;
        int totalCount = 0;
        long todayPrice = 0;
        long thisMonthPrice = 0;
        long totalPrice = 0;
        report.FirstTime = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
        report.LastTime = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
        using (var connection = _context.GetConnection())
        {
            //當日
            string sql = @"SELECT * FROM OrderList 
                           WHERE SellerAccount = @SellerAccount AND CONVERT(DATETIME,SUBSTRING(OrderDate,0,11),111) BETWEEN @FirstTime AND @LastTime";
            var orderList = connection.Query<OrderList>(sql, report).AsList();
            todayCount = orderList.Count();
            foreach (var item in orderList)
            {
                todayPrice += Convert.ToInt64(item.TotalPrice);
            }

            //當月
            report.FirstTime = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
            DateTime lastDay = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1).AddDays(-1);
            report.LastTime = lastDay.ToString("yyyy-MM-dd 23:59:59");

            sql = @"SELECT * FROM OrderList 
                           WHERE SellerAccount = @SellerAccount AND CONVERT(DATETIME,SUBSTRING(OrderDate,0,11),111) BETWEEN @FirstTime AND @LastTime";
            var thisMonthOrderList = connection.Query<OrderList>(sql, report).AsList();
            thisMonthCount = thisMonthOrderList.Count();
            foreach (var item in thisMonthOrderList)
            {
                thisMonthPrice += Convert.ToInt64(item.TotalPrice);
            }

            //全部
            sql = @"SELECT * FROM OrderList 
                           WHERE SellerAccount = @SellerAccount";
            var totalList = connection.Query<OrderList>(sql, report).AsList();
            totalCount = totalList.Count();
            foreach (var item in totalList)
            {
                totalPrice += Convert.ToInt64(item.TotalPrice);
            }

            sql = @"SELECT * FROM OrderDetail WHERE SellerAccount = @SellerAccount";
            var orderDetails = connection.Query<ShoppingCart>(sql, report).AsList();
            int notebook = 0;
            int ipad = 0;
            int other = 0;
            foreach (var item in orderDetails)
            {
                sql = @"SELECT * FROM Product_Index
                        WHERE Id=@ProductID";
                var product = connection.QueryFirstOrDefault<Product>(sql, item);
                switch (product.category)
                {
                    case "筆電":
                        notebook += 1*item.Count;
                        break;
                    case "平板電腦":
                        ipad += 1 * item.Count;
                        break;
                    case "其他":
                        other += 1 * item.Count;
                        break;
                    default:
                        break;
                }
            }
            //取消
            sql = @"SELECT * FROM OrderDetail
                     WHERE SellerAccount = @SellerAccount AND Status= N'已取消'";
            int calcelCount = connection.Query(sql, report).Count();
            ReportData result = new ReportData();
            result.ThisMonthCount = thisMonthCount;
            result.TodayCount = todayCount;
            result.TotalCount = totalCount;
            result.TodayPrice = todayPrice;
            result.ThisMonthPrice = thisMonthPrice;
            result.TotalPrice = totalPrice;
            result.Notebook = notebook;
            result.IPad = ipad;
            result.other = other;
            result.CalcelCount = calcelCount;
            return result;
        }
    }

    public int TotalOrder(ReportViewModel select)
    {
        int totalCount = 0;
        using (var connection = _context.GetConnection())
        {
            string sql = @"SELECT * FROM OrderList
                        WHERE SellerAccount=@SellerAccount AND CONVERT(DATETIME,SUBSTRING(OrderDate,0,11),111) BETWEEN @FirstTime AND @LastTime";
            totalCount = connection.Query(sql, select).Count();
            return totalCount;
        }
    }

    public long TotalPrice(ReportViewModel select)
    {
        long totalPrice = 0;
        using (var connection = _context.GetConnection())
        {
            string sql = @"SELECT * FROM OrderList
                        WHERE SellerAccount=@SellerAccount AND CONVERT(DATETIME,SUBSTRING(OrderDate,0,11),111) BETWEEN @FirstTime AND @LastTime";
            var orderList = connection.Query<OrderList>(sql, select).AsList();
            foreach (var item in orderList)
            {
                totalPrice += Convert.ToInt64(item.TotalPrice);
            }
            return totalPrice;
        }
    }
}
