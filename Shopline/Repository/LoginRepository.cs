using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Dapper;

public class LoginRepository
{
    private readonly DapperContext _context;

    public LoginRepository()
    {
        _context = new DapperContext();
    }

    /// <summary>
    /// 查是否已已存在
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool GetUserById(string userName)
    {
        bool result;
        using (var connection = _context.GetConnection())
        {
            string sql = "SELECT * FROM Login WHERE UserName = @UserName";
            var response = connection.QueryFirstOrDefault<User>(sql, new { UserName = userName });
            result = response == null ? true : false;


        }
        return result;
    }

    public int AddUser(User user)
    {
        using (var connection = _context.GetConnection())
        {
            string sql = "INSERT INTO Login (UserName,PassWord,SellerName) VALUES (@UserName,@PassWord,@SellerName)";
            var response = connection.Execute(sql, user);
            return response;
        }
    }

    public User EditFromLogin(string userName)
    {
        User result = new User();
        using (var connection = _context.GetConnection())
        {
            string sql = @"SELECT * FROM Login
                           WHERE Login.UserName=@UserName";
            
            result = connection.QuerySingleOrDefault<User>(sql, new { UserName = userName });
            
            return result;
        }
    }


    public User GetLogin(User member)
    {
        using (var connection = _context.GetConnection())
        {
            string sql = @"SELECT * FROM Login
                           WHERE UserName=@UserName AND PassWord= @PassWord";
            var result = connection.QueryFirstOrDefault<User>(sql, member);
            return result;
        }
    }

    public bool UpdateAccount(User edit)
    {
        using (var connection = _context.GetConnection())
        {
            string sql = @"UPDATE Login
                       SET UserName = @UserName,
                           PassWord = @PassWord,
                           SellerName = @SellerName
                       WHERE Id = @Id";
            var result = connection.Execute(sql, edit) > 0 ? true : false;

            return result;
        }
    }

    public int DeleteAccount(string delete)
    {
        using (var connection = _context.GetConnection())
        {
            string sql = @"DELETE FROM  Login
                           WHERE Id = @Id";
            var result = connection.Execute(sql, new { Id = delete });
            return result;
        }
    }
    // 根據需要添加更多的方法
}
