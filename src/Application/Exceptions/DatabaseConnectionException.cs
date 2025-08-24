namespace Application.Exceptions;

/// <summary>
/// Database bağlantı hataları için custom exception
/// </summary>
public sealed class DatabaseConnectionException : Exception
{
    public string Operation { get; }
    public string? ConnectionString { get; }
    public string? ServerName { get; }
    public string? DatabaseName { get; }

    public DatabaseConnectionException(string message, string operation, string? connectionString = null, string? serverName = null, string? databaseName = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Operation = operation;
        ConnectionString = connectionString;
        ServerName = serverName;
        DatabaseName = databaseName;
    }

    public DatabaseConnectionException(string message, string operation, Exception? innerException = null)
        : this(message, operation, null, null, null, innerException)
    {
    }

    public override string ToString()
    {
        var baseString = base.ToString();
        var additionalInfo = $"Operation: {Operation}";
        
        if (!string.IsNullOrEmpty(ServerName))
            additionalInfo += $", Server: {ServerName}";
        
        if (!string.IsNullOrEmpty(DatabaseName))
            additionalInfo += $", Database: {DatabaseName}";
        
        return $"{baseString}\nAdditional Info: {additionalInfo}";
    }
}
