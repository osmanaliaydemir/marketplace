namespace Application.Exceptions;

/// <summary>
/// Entity bulunamadığında fırlatılan exception
/// </summary>
public sealed class EntityNotFoundException : Exception
{
    public string EntityType { get; }
    public long EntityId { get; }
    public string? TableName { get; }

    public EntityNotFoundException(string entityType, long entityId, string? tableName = null)
        : base($"{entityType} with ID {entityId} was not found{(tableName != null ? $" in table {tableName}" : "")}")
    {
        EntityType = entityType;
        EntityId = entityId;
        TableName = tableName;
    }

    public EntityNotFoundException(string entityType, long entityId, string message, string? tableName = null)
        : base(message)
    {
        EntityType = entityType;
        EntityId = entityId;
        TableName = tableName;
    }

    public override string ToString()
    {
        var baseString = base.ToString();
        var additionalInfo = $"EntityType: {EntityType}, EntityId: {EntityId}";
        
        if (!string.IsNullOrEmpty(TableName))
            additionalInfo += $", Table: {TableName}";
        
        return $"{baseString}\nAdditional Info: {additionalInfo}";
    }
}
