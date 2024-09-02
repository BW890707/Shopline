using System.Collections.Generic;
using System.Data.Entity;


public class User
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string PassWord { get; set; }
    public string SellerName { get; set; }
    public string statue { get; set; }
}

public class LoginResult
{
    public List<User> Users { get; set; }
    public List<ProductDetail> ProductDetail { get; set; }

}
