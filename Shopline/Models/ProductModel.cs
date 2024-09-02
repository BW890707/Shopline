
using System.Collections.Generic;
using System.Web.UI.WebControls;

public class Product
{
    public string Id { get; set; }
    public string Image { get; set; }
    /// <summary>
    /// 商品名
    /// </summary>
    public string ProductName { get; set; } = null;
    public string UserName { get; set; }
    public string Price { get; set; }
    public string category { get; set; }
    public string Stock { get; set; }
    public string SellerName { get; set; }
    public int page { get; set; } = 1;
    public string Status { get; set; }
    public string Location { get; set; }
}

public class ProductResult
{
    public List<Product> Product { get; set; }
    public List<ProductDetail> ProductDetail { get; set; }
    public List<Login> Login { get; set; }
    public int TotalPage { get; set; }
    public int Page { get; set; }

}
