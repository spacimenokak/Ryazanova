using System.Data;

namespace projectTest.Data;

public interface IDbConnectionFactory
{
    IDbConnection GetConnection();
}

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IDbConnection _connection;

    public DbConnectionFactory(IDbConnection connection)
    {
        _connection = connection;
    }

    public IDbConnection GetConnection()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
        return _connection;
    }
}
