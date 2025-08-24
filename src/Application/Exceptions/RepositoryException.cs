namespace Application.Exceptions;

/// <summary>
/// Repository layer'da oluşan hatalar için custom exception
/// </summary>
public sealed class RepositoryException : Exception
{
    public string Operation { get; }
    public string EntityType { get; }
    public long? EntityId { get; }
    public string? TableName { get; }

    public RepositoryException(string message, string operation, string entityType, long? entityId = null, string? tableName = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Operation = operation;
        EntityType = entityType;
        EntityId = entityId;
        TableName = tableName;
    }

    public RepositoryException(string message, string operation, string entityType, Exception? innerException = null)
        : this(message, operation, entityType, null, null, innerException)
    {
    }

    public override string ToString()
    {
        var baseString = base.ToString();
        var additionalInfo = $"Operation: {Operation}, EntityType: {EntityType}";
        
        if (EntityId.HasValue)
            additionalInfo += $", EntityId: {EntityId}";
        
        if (!string.IsNullOrEmpty(TableName))
            additionalInfo += $", Table: {TableName}";
        
        return $"{baseString}\nAdditional Info: {additionalInfo}";
    }
}
