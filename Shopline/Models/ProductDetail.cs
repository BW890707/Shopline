using System.Collections.Generic;

public class ProductDetail {
    public string Id { get; set; }
    //物品狀況:
    public string Status { get; set; }
    //物品所在地:
    public string Location { get; set; }
    //最新關注時間:
    public string AttentionTime { get; set; }
    //上架時間
    public string AddedTime { get; set; }
    //價格更新時間
    public string UpdateTime { get; set; }
    //庫存:
    public string Stock { get; set; }
    //賣家帳號:
    public string SellerAccount { get; set; }
    //全部商品
    public string AllProduct { get; set; }
    //賣場評價
    public string Assess { get; set; }
    //商品名稱
    public string ProductName { get; set; } = null;
    //價格
    public string Price { get; set; }
    //相片    
    public string Image { get; set; }
}

