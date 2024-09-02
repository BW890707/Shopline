using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public class DapperContext : IDisposable
{
    private readonly IDbConnection _dbConnection;
    private readonly string _connectionString;

    public DapperContext()
    {
        _connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
    }

    public IDbConnection GetConnection()
    {
        var dbConnection = new SqlConnection(_connectionString);
        dbConnection.Open();

        return dbConnection;
    }

    public void Dispose()
    {
        if (_dbConnection != null && _dbConnection.State == ConnectionState.Open)
        {
            _dbConnection.Close();
        }
    }
}