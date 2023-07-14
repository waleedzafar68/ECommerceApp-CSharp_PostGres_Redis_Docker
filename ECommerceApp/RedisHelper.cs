namespace ECommerceApp;
using StackExchange.Redis;
using System;

public class RedisHelper
{
    private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
        string connectionString = "172.17.0.2:6379"; // Replace with your Redis connection string
        return ConnectionMultiplexer.Connect(connectionString);
    });

    public static ConnectionMultiplexer Connection
    {
        get
        {
            return lazyConnection.Value;
        }
    }

    public static IDatabase RedisCache
    {
        get
        {
            return Connection.GetDatabase();
        }
    }
}
