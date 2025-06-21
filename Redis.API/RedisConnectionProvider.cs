using StackExchange.Redis;

namespace Redis.API;

public class RedisConnectionProvider : IDisposable
{
    private readonly string _path;
    private FileSystemWatcher _watcher;
    private ConnectionMultiplexer _connection;
    private readonly object _lock = new();
    private bool _disposed = false;

    public RedisConnectionProvider(string path)
    {
        _path = path;
        Connect();

        _watcher = new FileSystemWatcher(Path.GetDirectoryName(_path) ?? ".", Path.GetFileName(_path));
        _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
        _watcher.Changed += OnChanged;
        _watcher.EnableRaisingEvents = true;
    }

    private void Connect()
    {
        var connStr = File.ReadAllText(_path).Trim();
        _connection = ConnectionMultiplexer.Connect(connStr);
        Console.WriteLine("Conectado ao Redis com a connection string atual.");
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        lock (_lock)
        {
            try
            {
                Console.WriteLine("Arquivo de conexão Redis alterado. Reconectando...");
                _connection.Dispose();
                Thread.Sleep(500);
                Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao reconectar: {ex.Message}");
            }
        }
    }

    public ConnectionMultiplexer GetConnection()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RedisConnectionProvider));

        return _connection;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _watcher?.Dispose();
        _connection?.Dispose();
    }
}
