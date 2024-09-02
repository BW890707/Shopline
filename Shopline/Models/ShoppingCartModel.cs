
using System.Collections.Generic;
using System.Web.UI.WebControls;

public class ShoppingCart
{
    public string ID { get; set; } 
    public string ProductName { get; set; }
    public int Price { get; set; }
    public int Count { get; set; }
    public string SellerName { get; set; }
    public string SellerAccount { get; set; }
    public string OrderNumber { get; set; }
    public string ProductID { get; set; }
    public string BuyerName { get; set; }
    public string OrderID { get; set; }

    public string Status { get; set; }
    public string Image { get; set; }
}

public class OrderList
{
    public string OrderNumber { get; set; }
    public string OrderDate { get; set; }
    public string TotalPrice { get; set; }
    public string BuyerName { get; set; }
    public string SellerAccount { get; set; }
}

//一筆訂單
public class OrderResult
{
    public OrderList OrderModel { get; set; }

    public List<ShoppingCart> OrderValues { get; set; }
}