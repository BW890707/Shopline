using System;
using System.Runtime.CompilerServices;

public class CreateProduct
{
    public string Id { get; set; }
    public string ProductName { get; set; }
    public string SellerName { get; set; }
    public string Price { get; set; }
    public string category { get; set; }
    public string Stock { get; set; }
    public string Location { get; set; }
    public string Image { get; set; }
    public string Status { get; set; }
    public string AttentionTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public string AddedTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public string UpdateTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}

public class EditProduct
{
    public string Id { get; set; }
    public string Image { get; set; }
    public string ProductName { get; set; }
    public string SellerName { get; set; }
    public string Price { get; set; }
    public string Category { get; set; }
    public string Status { get; set; }
    public string Location { get; set; }
    public int Stock { get; set; }
}