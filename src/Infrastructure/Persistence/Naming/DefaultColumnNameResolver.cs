using Infrastructure.Persistence.Naming;

namespace Infrastructure.Persistence.Naming;

public sealed class DefaultColumnNameResolver : IColumnNameResolver
{
    public string ResolveColumnName(string propertyName)
    {
        // Simple passthrough - property name equals column name
        return propertyName;
    }

    public string ResolveIdColumnName(Type entityType)
    {
        // Default ID column name
        return "Id";
    }
}
