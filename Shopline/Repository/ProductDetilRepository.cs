using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Dapper;

public class ProductDetilRepository
{
    private readonly DapperContext _context;

    public ProductDetilRepository()
    {
        _context = new DapperContext();
    }
    /// <summary>
    /// 查詢
    /// </summary>
    /// <returns></returns>
    public ProductResult GetProducts(Product product = null)
    {
        ProductResult result = new ProductResult();
        List<Product> productList = new List<Product>();
        using (var connection = _context.GetConnection())
        {
            string sql = @"SELECT Product_Index.Id,
                                  Product_Index.Image,
                                  Product_Index.ProductName,
                                  Product_Index.Price,
                                  Product_Index.category,
                                  Login.SellerName
                                  FROM Product_Index
                                  JOIN Login
                                  ON Product_Index.UserName = Login.UserName
                                  WHERE 1=1 ";
            if (product != null)//若有輸入關鍵字
            {
                sql += "AND ProductName LIKE CONCAT('%',@ProductName,'%')";
                productList = connection.Query<Product>(sql, new { ProductName = product.ProductName }).AsList();
            }
            else
            {
                productList = connection.Query<Product>(sql).AsList();
            }
            result.Product = productList;
            result.TotalPage = productList.Count / 6;
            if (productList.Count % 6 != 0)
            {
                result.TotalPage++;
            }
            return result;
        }
    }

    public ProductResult GetProductDetail(string Id)
    {
        ProductResult result = new ProductResult();
        List<ProductDetail> productList = new List<ProductDetail>();
        using (var connection = _context.GetConnection())
        {
            string sql = @"SELECT Product_Index.ProductName,
                            Product_Index.Price,
                            Product_Index.Image,
                            Product_Detail.Id,
                            Product_Detail.Status,
                            Product_Detail.Location,
                            Product_Detail.AttentionTime,
                            Product_Detail.AddedTime,
                            Product_Detail.UpdateTime,
                            Product_Detail.Stock,
                            Product_Detail.SellerAccount,
                            Product_Detail.AllProduct,
                            Product_Detail.Assess
                            FROM Product_Index join Product_Detail on  Product_Detail.Id
                            = Product_Index.Id
                            WHERE Product_Index.Id=@Id";
            productList = connection.Query<ProductDetail>(sql, new { Id = Id }).AsList();
            result.ProductDetail = productList;
            result.TotalPage = productList.Count / 6;
            return result;
        }
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="create"></param>
    /// <returns></returns>
    public bool CreateProduct(CreateProduct create)
    {
        using (var connection = _context.GetConnection())
        {
            string sql = @"INSERT INTO Product_Index (Id,Image, ProductName, UserName, Price, category)
                           VALUES  (@Id, @Image ,@ProductName ,@SellerName ,@Price, @category)";
            var result = connection.Execute(sql, create) > 0 ? true : false;

            sql = @"INSERT INTO Product_Detail (Id,Status,Location,Stock,AttentionTime,AddedTime,UpdateTime,SellerAccount)
                           VALUES  (@Id,@Status,@Location,@Stock,@AttentionTime,@AddedTime,@UpdateTime,@SellerName)";
            result = connection.Execute(sql, create) > 0 ? true : false;
            return result;
        }
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    public bool UpdateProduct(EditProduct edit)
    {
        using (var connection = _context.GetConnection())
        {
            string sql = @"UPDATE Product_Index
                       SET Image = @Image,
                           ProductName = @ProductName,
                           Price = @Price,
                           Category = @Category
                       WHERE Id = @Id";
            var result = connection.Execute(sql, edit) > 0 ? true : false;

            sql = @"UPDATE Product_Detail
                SET Status = @Status,
                    Location = @Location,
                    Stock = @Stock
                WHERE Id = @Id";

            result = connection.Execute(sql, edit) > 0 ? true : false;

            return result;
        }
    }

    public List<Product> EditFromProducts(string id = null, string userName = null)
    {
        List<Product> result = new List<Product>();
        using (var connection = _context.GetConnection())
        {
            string sql = @"SELECT Product_Index.ProductName,
                            Product_Index.Price,
                            Product_Index.Image,
                            Product_Index.category,
                            Product_Index.UserName,
                            Product_Detail.Id,
                            Product_Detail.Stock
                            FROM Product_Index left join Product_Detail on  Product_Detail.Id
                            = Product_Index.Id ";
            if (id != null)
            {
                sql += "WHERE Product_Index.Id=@Id";
                result = connection.Query<Product>(sql, new { Id = id }).ToList();
            }
            else if (userName != null)
            {
                sql += "WHERE Product_Index.UserName=@UserName";
                result = connection.Query<Product>(sql, new { UserName = userName }).ToList();
            }
            else
            {
                result = connection.Query<Product>(sql).ToList();
            }
            return result;
        }
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    public int DeleteProduct(string Id)
    {
        using (var connection = _context.GetConnection())
        {
            string sql = @"DELETE FROM  Product_Index
                           WHERE Id = @Id";
            var result = connection.Execute(sql, new { Id = Id });

            sql = @"DELETE FROM  Product_Detail
                           WHERE Id = @Id";
            result = connection.Execute(sql, new { Id = Id });
            return result;
        }
    }
    //20240712 待修改
    public List<Product> KeyWordSearch(Product KeyWord)
    {
        using (var connection = _context.GetConnection())
        {
            string sql = @"SELECT *
            FROM  Product_Index
            WHERE ProductName LIKE CONCAT('%',@ProductName,'%')";
            var result = connection.Query<Product>(sql, KeyWord).ToList();
            return result;
        }
    }
}
